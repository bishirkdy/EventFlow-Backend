using EventFlow.Registration.Application.Abstractions.Persistence;


namespace EventFlow.Registration.Infrastructure.Persistence.Repositories;

public sealed class RegistrationFormRepository(RegistrationDbContext db): IRegistrationFormRepository
{
    public async Task<RegistrationForm?> GetByEventIdAsync(Guid eventId, bool includeFields = false, bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        IQueryable<RegistrationForm> query = db.RegistrationForms;

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        if (includeFields)
        {
            query = query.Include(x => x.Fields);
        }

        return await query.SingleOrDefaultAsync(
            x => x.EventId == eventId, cancellationToken);
    }

    public void Add(RegistrationForm form)
    {
        db.RegistrationForms.Add(form);
    }

    public void ReplaceFields(RegistrationForm form, IEnumerable<RegistrationFormField> fields)
    {
        var incoming = fields.ToList();
        var incomingIds = incoming
            .Where(x => x.Id != Guid.Empty)
            .Select(x => x.Id)
            .ToHashSet();

        var existing = db.RegistrationFormFields
            .Where(x => x.RegistrationFormId == form.Id)
            .ToList();

        db.RegistrationFormFields.RemoveRange(
            existing.Where(x => !incomingIds.Contains(x.Id)));

        foreach (var field in incoming)
        {
            var current = existing.FirstOrDefault(x => x.Id == field.Id);

            if (current is null)
            {
                field.Id = Guid.NewGuid();
                field.RegistrationFormId = form.Id;
                db.RegistrationFormFields.Add(field);
                continue;
            }

            current.FieldKey = field.FieldKey;
            current.Label = field.Label;
            current.FieldType = field.FieldType;
            current.IsRequired = field.IsRequired;
            current.DisplayOrder = field.DisplayOrder;
            current.OptionsJson = field.OptionsJson;
            current.ValidationJson = field.ValidationJson;
        }
    }
}
