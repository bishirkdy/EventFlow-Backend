namespace EventFlow.Registration.Infrastructure.Services;

public static class ImageDimensionReader
{
    public const int PngSignature = unchecked((int)0x89504E47);
    public const int JpegSignature = 0xFFD8FF;
    public const int RiffSignature = 0x52494646;

    public static bool HasSupportedSignature(ReadOnlySpan<byte> header)
    {
        if (header.Length < 12)
        {
            return false;
        }

        if (ReadInt32BigEndian(header) == PngSignature)
        {
            return true;
        }

        if (header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF)
        {
            return true;
        }

        if (ReadInt32BigEndian(header) == RiffSignature &&
            header[8] == (byte)'W' && header[9] == (byte)'E' &&
            header[10] == (byte)'B' && header[11] == (byte)'P')
        {
            return true;
        }

        return false;
    }

    public static (int Width, int Height) Read(ReadOnlySpan<byte> data)
    {
        if (data.Length < 24)
        {
            throw new InvalidDataException("Image data is too short to contain dimensions.");
        }

        if (ReadInt32BigEndian(data) == PngSignature)
        {
            return (
                ReadInt32BigEndian(data.Slice(16, 4)),
                ReadInt32BigEndian(data.Slice(20, 4)));
        }

        if (data[0] == 0xFF && data[1] == 0xD8 && data[2] == 0xFF)
        {
            return ReadJpegDimensions(data);
        }

        if (ReadInt32BigEndian(data) == RiffSignature)
        {
            return ReadWebpDimensions(data);
        }

        throw new InvalidDataException("Unsupported image format.");
    }

    private static (int Width, int Height) ReadJpegDimensions(ReadOnlySpan<byte> data)
    {
        var offset = 2;

        while (offset + 9 < data.Length)
        {
            if (data[offset] != 0xFF)
            {
                offset++;
                continue;
            }

            var marker = data[offset + 1];

            if (marker == 0xFF)
            {
                offset++;
                continue;
            }

            if (marker == 0xD8 || marker == 0x01 || (marker >= 0xD0 && marker <= 0xD7))
            {
                offset += 2;
                continue;
            }

            var segmentLength = (data[offset + 2] << 8) | data[offset + 3];

            if (IsStartOfFrame(marker))
            {
                var height = (data[offset + 5] << 8) | data[offset + 6];
                var width = (data[offset + 7] << 8) | data[offset + 8];
                return (width, height);
            }

            if (segmentLength < 2)
            {
                break;
            }

            offset += 2 + segmentLength;
        }

        throw new InvalidDataException("Could not read JPEG dimensions.");
    }

    private static (int Width, int Height) ReadWebpDimensions(ReadOnlySpan<byte> data)
    {
        var chunk = System.Text.Encoding.ASCII.GetString(data.Slice(12, 4));

        var payload = 20;

        if (chunk == "VP8X")
        {
            var width = 1 + (data[payload + 4] | (data[payload + 5] << 8) | (data[payload + 6] << 16));
            var height = 1 + (data[payload + 7] | (data[payload + 8] << 8) | (data[payload + 9] << 16));
            return (width, height);
        }

        if (chunk == "VP8L")
        {
            var bits = data[payload + 1] | (data[payload + 2] << 8) | (data[payload + 3] << 16) | (data[payload + 4] << 24);
            var width = (bits & 0x3FFF) + 1;
            var height = ((bits >> 14) & 0x3FFF) + 1;
            return (width, height);
        }

        if (chunk == "VP8 ")
        {
            var frameStart = payload + 3;

            if (data[frameStart] != 0x9D || data[frameStart + 1] != 0x01 || data[frameStart + 2] != 0x2A)
            {
                throw new InvalidDataException("Invalid WebP frame header.");
            }

            var width = (data[frameStart + 3] | (data[frameStart + 4] << 8)) & 0x3FFF;
            var height = (data[frameStart + 5] | (data[frameStart + 6] << 8)) & 0x3FFF;
            return (width, height);
        }

        throw new InvalidDataException("Unsupported WebP chunk.");
    }

    private static bool IsStartOfFrame(byte marker) =>
        marker >= 0xC0 && marker <= 0xCF &&
        marker != 0xC4 && marker != 0xC8 && marker != 0xCC;

    private static int ReadInt32BigEndian(ReadOnlySpan<byte> data) =>
        (data[0] << 24) | (data[1] << 16) | (data[2] << 8) | data[3];
}
