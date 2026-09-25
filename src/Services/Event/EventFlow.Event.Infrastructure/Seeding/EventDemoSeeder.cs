using EventFlow.Event.Domain.Entities;
using EventFlow.Event.Domain.Enums;
using EventFlow.Event.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Event.Infrastructure.Seeding;

public static class EventDemoSeeder
{
    private const string CreatedBy = "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa";

    private static readonly string WeddingHero = "https://images.unsplash.com/photo-1519741497674-611481863552?auto=format&fit=crop&w=1600&q=85";
    private static readonly string ConferenceHero = "https://images.unsplash.com/photo-1540575467063-178a50c2df87?auto=format&fit=crop&w=1600&q=85";
    private static readonly string EducationHero = "https://images.unsplash.com/photo-1524178232363-1fb2b075b655?auto=format&fit=crop&w=1600&q=85";
    private static readonly string FestivalHero = "https://images.unsplash.com/photo-1501386761578-eac5c94b800a?auto=format&fit=crop&w=1600&q=85";
    private static readonly string SportsHero = "https://images.unsplash.com/photo-1461896836934-ffe607ba8211?auto=format&fit=crop&w=1600&q=85";

    private static readonly string VenueImage = "https://images.unsplash.com/photo-1497366811353-6870744d04b2?auto=format&fit=crop&w=1400&q=85";
    private static readonly string WeddingVenueImage = "https://images.unsplash.com/photo-1519167758481-83f550bb49b3?auto=format&fit=crop&w=1400&q=85";
    private static readonly string SportsVenueImage = "https://images.unsplash.com/photo-1459865264687-595d652de67e?auto=format&fit=crop&w=1400&q=85";

    public static async Task SeedAsync(EventCoreDbContext db, CancellationToken cancellationToken = default)
    {
        if (await db.EventEntities.AnyAsync(cancellationToken))
            return;

        var eventTypes = await db.EventTypes.ToDictionaryAsync(x => x.Code, cancellationToken);
        var eventTypeFeatures = await db.EventTypeFeatures.ToListAsync(cancellationToken);
        var featureById = await db.Features.ToDictionaryAsync(x => x.Id, cancellationToken);

        var wedding = CreateEvent(
            "Aarav & Meera — Wedding Celebration",
            "An intimate three-day wedding celebration with family, friends and cherished traditions.",
            eventTypes[EventTypeCode.Wedding].Id,
            "Private Wedding",
            new DateTime(2026, 12, 12, 10, 0, 0, DateTimeKind.Utc),
            new DateTime(2026, 12, 12, 20, 30, 0, DateTimeKind.Utc));

        var conference = CreateEvent(
            "EventFlow Summit 2027",
            "A two-day conference for product builders, engineers, designers and event technology leaders.",
            eventTypes[EventTypeCode.Conference].Id,
            "Technology Conference",
            new DateTime(2027, 2, 18, 3, 30, 0, DateTimeKind.Utc),
            new DateTime(2027, 2, 19, 13, 0, 0, DateTimeKind.Utc));

        var education = CreateEvent(
            "Future Skills Learning Week",
            "A practical learning program covering software engineering, data and modern product development.",
            eventTypes[EventTypeCode.Education].Id,
            "Professional Learning",
            new DateTime(2027, 3, 8, 3, 30, 0, DateTimeKind.Utc),
            new DateTime(2027, 3, 12, 13, 0, 0, DateTimeKind.Utc));

        var festival = CreateEvent(
            "Malabar Arts & Culture Festival",
            "A vibrant celebration of music, performance, food, craft and regional culture.",
            eventTypes[EventTypeCode.Festival].Id,
            "Cultural Festival",
            new DateTime(2027, 4, 10, 4, 0, 0, DateTimeKind.Utc),
            new DateTime(2027, 4, 12, 17, 0, 0, DateTimeKind.Utc));

        var sports = CreateEvent(
            "Kerala Interclub Sports Meet",
            "A multi-day competition bringing together clubs and athletes across track, court and field events.",
            eventTypes[EventTypeCode.Sports].Id,
            "Interclub Championship",
            new DateTime(2027, 5, 6, 3, 30, 0, DateTimeKind.Utc),
            new DateTime(2027, 8, 7, 13, 0, 0, DateTimeKind.Utc));

        var events = new[] { wedding, conference, education, festival, sports };
        db.EventEntities.AddRange(events);

        foreach (var eventEntity in events)
        {
            foreach (var typeFeature in eventTypeFeatures.Where(x => x.EventTypeId == eventEntity.EventTypeId))
            {
                db.EventFeatures.Add(new EventFeature(
                    eventEntity.Id,
                    typeFeature.FeatureId,
                    typeFeature.IsEnabledByDefault));
            }
        }

        AddEventImages(db, wedding, new[] { WeddingHero, WeddingVenueImage, "https://images.unsplash.com/photo-1511285560929-80b456fea0bc?auto=format&fit=crop&w=1400&q=85" });
        AddEventImages(db, conference, new[] { ConferenceHero, VenueImage, "https://images.unsplash.com/photo-1556761175-b413da4baf72?auto=format&fit=crop&w=1400&q=85" });
        AddEventImages(db, education, new[] { EducationHero, VenueImage, "https://images.unsplash.com/photo-1523240795612-9a054b0db644?auto=format&fit=crop&w=1400&q=85" });
        AddEventImages(db, festival, new[] { FestivalHero, "https://images.unsplash.com/photo-1533174072545-7a4b6ad7a6c3?auto=format&fit=crop&w=1400&q=85", "https://images.unsplash.com/photo-1506157786151-b8491531f063?auto=format&fit=crop&w=1400&q=85" });
        AddEventImages(db, sports, new[] { SportsHero, SportsVenueImage, "https://images.unsplash.com/photo-1517649763962-0c623066013b?auto=format&fit=crop&w=1400&q=85" });

        AddWedding(db, wedding);
        AddConference(db, conference);
        AddEducation(db, education);
        AddFestival(db, festival);
        AddSports(db, sports);

        await db.SaveChangesAsync(cancellationToken);
    }

    private static EventEntity CreateEvent(string name, string description, Guid eventTypeId, string subtype, DateTime start, DateTime end)
    {
        var eventEntity = new EventEntity(name, description, eventTypeId, subtype, start, end, "Asia/Kolkata", Guid.Parse(CreatedBy));
        eventEntity.Publish();
        return eventEntity;
    }

    private static void AddEventImages(EventCoreDbContext db, EventEntity eventEntity, IEnumerable<string> urls)
    {
        var order = 0;
        foreach (var url in urls)
        {
            db.EventImages.Add(new EventImage(
                eventEntity.Id,
                url,
                $"demo/{eventEntity.Id}/{order}.jpg",
                $"event-{order + 1}.jpg",
                "image/jpeg",
                1_000_000 + order,
                order++));
        }
    }

    private static void AddWedding(EventCoreDbContext db, EventEntity e)
    {
        var venue = new Venue(e.Id, "The Garden Courtyard", "An elegant private garden venue with an open-air celebration lawn.", "Kozhikode, Kerala", 350, WeddingVenueImage);
        db.Venues.Add(venue);

        var page = AddPage(db, e, "Home", "home", "Home", 1);
        AddSections(db, page, new[]
        {
            ("hero", "Aarav & Meera", "Two families, one beautiful beginning.", WeddingHero),
            ("couple", "The Couple", "Aarav and Meera invite you to celebrate their wedding journey with the people they love most.", "https://images.unsplash.com/photo-1511285560929-80b456fea0bc?auto=format&fit=crop&w=1200&q=85"),
            ("details", "Wedding Details", "12 December 2026 · Ceremony 4:30 PM · Reception 7:00 PM", WeddingHero),
            ("venue", "The Garden Courtyard", "Kozhikode, Kerala · A relaxed garden celebration surrounded by family and friends.", WeddingVenueImage),
            ("gallery", "Moments", "A small collection of memories from the journey to the celebration.", WeddingHero),
            ("rsvp", "RSVP", "Please confirm your attendance so we can prepare a place for you at the celebration.", null)
        });
        AddNavigation(db, e, page);
    }

    private static void AddConference(EventCoreDbContext db, EventEntity e)
    {
        var main = new Venue(e.Id, "Summit Convention Hall", "The main conference auditorium for keynotes and plenary sessions.", "Kochi, Kerala", 1200, VenueImage);
        var workshop = new Venue(e.Id, "Innovation Studio", "Flexible workshop space for hands-on sessions and roundtables.", "Kochi, Kerala", 240, "https://images.unsplash.com/photo-1497366754035-f200968a6e72?auto=format&fit=crop&w=1400&q=85");
        db.Venues.AddRange(main, workshop);

        var opening = new Section(e.Id, "Main Stage", "Keynotes, leadership conversations and conference-wide sessions.", 1);
        var workshops = new Section(e.Id, "Workshops", "Hands-on technical and product sessions.", 2);
        db.Sections.AddRange(opening, workshops);

        AddSession(db, e, opening, "Opening Keynote: Building for the Next Million Users", "Keynote", 2027, 2, 18, 4, 0, 5, 0, main, ConferenceHero);
        AddSession(db, e, opening, "The Architecture Behind Reliable Event Platforms", "Talk", 2027, 2, 18, 6, 0, 7, 0, main, "https://images.unsplash.com/photo-1556761175-b413da4baf72?auto=format&fit=crop&w=1200&q=85");
        AddSession(db, e, workshops, "Designing Event Experiences with Data", "Workshop", 2027, 2, 18, 8, 0, 9, 30, workshop, "https://images.unsplash.com/photo-1516321318423-f06f85e504b3?auto=format&fit=crop&w=1200&q=85");
        AddSession(db, e, workshops, "Modern APIs, Messaging and Distributed Workflows", "Workshop", 2027, 2, 19, 5, 0, 6, 30, workshop, "https://images.unsplash.com/photo-1558494949-ef010cbdcc31?auto=format&fit=crop&w=1200&q=85");
        AddConferencePeople(db, e);

        AddPageSet(db, e, new[]
        {
            ("Home", "home", ConferenceHero, "Welcome to EventFlow Summit 2027", "Two days of practical ideas, technical depth and meaningful conversations."),
            ("Schedule", "schedule", "https://images.unsplash.com/photo-1540575467063-178a50c2df87?auto=format&fit=crop&w=1400&q=85", "Conference Schedule", "Explore keynotes, workshops and conversations across the two-day program."),
            ("Speakers", "speakers", "https://images.unsplash.com/photo-1560250097-0b93528c311a?auto=format&fit=crop&w=1400&q=85", "Speakers", "Meet practitioners and leaders sharing real lessons from the field."),
            ("Sponsors", "sponsors", "https://images.unsplash.com/photo-1556761175-4b46a572b786?auto=format&fit=crop&w=1400&q=85", "Our Sponsors", "Partners supporting the event and the community behind it."),
            ("Venue", "venue", VenueImage, "Summit Convention Hall", "Kochi, Kerala — with a dedicated auditorium and workshop spaces."),
            ("Registration", "registration", ConferenceHero, "Join the Summit", "Reserve your place and receive the complete event schedule."),
        });
        AddNavigation(db, e, db.EventPages.Local.Where(x => x.EventId == e.Id).OrderBy(x => x.DisplayOrder).ToArray());
    }

    private static void AddEducation(EventCoreDbContext db, EventEntity e)
    {
        var campus = new Venue(e.Id, "Learning Hub", "A modern learning space with classrooms, labs and discussion areas.", "Thrissur, Kerala", 500, VenueImage);
        var lab = new Venue(e.Id, "Technology Lab", "A practical lab for coding, data and project-based workshops.", "Thrissur, Kerala", 120, "https://images.unsplash.com/photo-1531482615713-2afd69097998?auto=format&fit=crop&w=1400&q=85");
        db.Venues.AddRange(campus, lab);
        var fundamentals = new Section(e.Id, "Core Skills", "Foundational sessions for modern software development.", 1);
        var advanced = new Section(e.Id, "Advanced Practice", "Hands-on workshops and project sessions.", 2);
        db.Sections.AddRange(fundamentals, advanced);
        AddSession(db, e, fundamentals, "Software Engineering Foundations", "Lecture", 2027, 3, 8, 4, 0, 5, 30, campus, EducationHero);
        AddSession(db, e, fundamentals, "Building Production-Ready APIs", "Workshop", 2027, 3, 9, 4, 0, 5, 30, lab, "https://images.unsplash.com/photo-1516321318423-f06f85e504b3?auto=format&fit=crop&w=1200&q=85");
        AddSession(db, e, advanced, "Distributed Systems Lab", "Lab", 2027, 3, 10, 6, 0, 7, 30, lab, "https://images.unsplash.com/photo-1558494949-ef010cbdcc31?auto=format&fit=crop&w=1200&q=85");
        AddSession(db, e, advanced, "Career Portfolio Workshop", "Workshop", 2027, 3, 11, 6, 0, 7, 0, campus, "https://images.unsplash.com/photo-1521737711867-e3b97375f902?auto=format&fit=crop&w=1200&q=85");
        AddPageSet(db, e, new[]
        {
            ("Home", "home", EducationHero, "Future Skills Learning Week", "Five days of focused learning, practice and feedback."),
            ("Learning", "learning", "https://images.unsplash.com/photo-1523240795612-9a054b0db644?auto=format&fit=crop&w=1400&q=85", "Learn by Building", "Practical sessions designed around projects and real development workflows."),
            ("Schedule", "schedule", EducationHero, "Learning Schedule", "Plan your week across lectures, labs and workshops."),
            ("Certificates", "certificates", "https://images.unsplash.com/photo-1589330694653-ded6df03f754?auto=format&fit=crop&w=1400&q=85", "Certificates", "Completion certificates are available for eligible participants."),
            ("Feedback", "feedback", "https://images.unsplash.com/photo-1556761175-b413da4baf72?auto=format&fit=crop&w=1400&q=85", "Tell Us What You Learned", "Share feedback to improve future learning programs."),
        });
        AddNavigation(db, e, db.EventPages.Local.Where(x => x.EventId == e.Id).OrderBy(x => x.DisplayOrder).ToArray());
    }

    private static void AddFestival(EventCoreDbContext db, EventEntity e)
    {
        var grounds = new Venue(e.Id, "Festival Grounds", "The central outdoor venue for performances, food and community activities.", "Kozhikode, Kerala", 5000, "https://images.unsplash.com/photo-1533174072545-7a4b6ad7a6c3?auto=format&fit=crop&w=1400&q=85");
        var stage = new Venue(e.Id, "Heritage Stage", "An open-air performance stage for music and cultural programs.", "Kozhikode, Kerala", 1800, "https://images.unsplash.com/photo-1506157786151-b8491531f063?auto=format&fit=crop&w=1400&q=85");
        db.Venues.AddRange(grounds, stage);
        var culture = new Section(e.Id, "Cultural Programs", "Music, dance and heritage performances.", 1);
        var community = new Section(e.Id, "Community", "Food, craft and community experiences.", 2);
        db.Sections.AddRange(culture, community);
        AddSession(db, e, culture, "Opening Cultural Showcase", "Performance", 2027, 4, 10, 6, 0, 7, 0, stage, FestivalHero);
        AddSession(db, e, culture, "Contemporary Folk Ensemble", "Performance", 2027, 4, 11, 8, 0, 9, 30, stage, "https://images.unsplash.com/photo-1506157786151-b8491531f063?auto=format&fit=crop&w=1200&q=85");
        AddSession(db, e, community, "Local Food Stories", "Talk", 2027, 4, 11, 10, 0, 11, 0, grounds, "https://images.unsplash.com/photo-1515003197210-e0cd71810b5f?auto=format&fit=crop&w=1200&q=85");
        AddSession(db, e, community, "Craft and Design Market Walk", "Experience", 2027, 4, 12, 6, 0, 8, 0, grounds, "https://images.unsplash.com/photo-1528698827591-e19ccd7bc23d?auto=format&fit=crop&w=1200&q=85");
        AddFestivalSponsors(db, e);
        AddPageSet(db, e, new[]
        {
            ("Home", "home", FestivalHero, "Malabar Arts & Culture Festival", "Three days of music, performance, food and community."),
            ("Schedule", "schedule", FestivalHero, "Festival Schedule", "Follow the performances, talks and experiences across the festival grounds."),
            ("Gallery", "gallery", "https://images.unsplash.com/photo-1501386761578-eac5c94b800a?auto=format&fit=crop&w=1400&q=85", "Festival Moments", "A visual celebration of Malabar's creative spirit."),
            ("Sponsors", "sponsors", "https://images.unsplash.com/photo-1556761175-4b46a572b786?auto=format&fit=crop&w=1400&q=85", "Festival Partners", "Partners helping bring the festival to life."),
            ("Venue", "venue", "https://images.unsplash.com/photo-1533174072545-7a4b6ad7a6c3?auto=format&fit=crop&w=1400&q=85", "Festival Grounds", "Kozhikode, Kerala."),
        });
        AddNavigation(db, e, db.EventPages.Local.Where(x => x.EventId == e.Id).OrderBy(x => x.DisplayOrder).ToArray());
    }

    private static void AddSports(EventCoreDbContext db, EventEntity e)
    {
        var stadium = new Venue(e.Id, "City Sports Arena", "The main arena for championship matches and opening ceremonies.", "Kochi, Kerala", 8000, SportsVenueImage);
        var court = new Venue(e.Id, "Indoor Court Complex", "Indoor courts for club competitions and training sessions.", "Kochi, Kerala", 1200, "https://images.unsplash.com/photo-1504450758481-7338eba7524a?auto=format&fit=crop&w=1400&q=85");
        db.Venues.AddRange(stadium, court);
        var track = new Section(e.Id, "Track & Field", "Athletics and field competitions.", 1);
        var team = new Section(e.Id, "Team Events", "Club team competitions and finals.", 2);
        db.Sections.AddRange(track, team);
        AddSession(db, e, track, "Opening Athletics Session", "Competition", 2027, 5, 6, 5, 0, 7, 0, stadium, SportsHero);
        AddSession(db, e, track, "100m & 400m Finals", "Competition", 2027, 5, 7, 5, 30, 7, 30, stadium, "https://images.unsplash.com/photo-1461896836934-ffe607ba8211?auto=format&fit=crop&w=1200&q=85");
        AddSession(db, e, team, "Club Basketball Semi-Final", "Match", 2027, 5, 7, 8, 0, 9, 30, court, "https://images.unsplash.com/photo-1518065896235-a4c93e088e7b?auto=format&fit=crop&w=1200&q=85");
        AddSession(db, e, team, "Championship Final", "Match", 2027, 5, 8, 8, 0, 10, 0, stadium, "https://images.unsplash.com/photo-1517649763962-0c623066013b?auto=format&fit=crop&w=1200&q=85");
        AddPageSet(db, e, new[]
        {
            ("Home", "home", SportsHero, "Kerala Interclub Sports Meet", "Competition, community and a full week of sport."),
            ("Schedule", "schedule", SportsHero, "Competition Schedule", "Find fixtures, finals and championship sessions."),
            ("Gallery", "gallery", "https://images.unsplash.com/photo-1517649763962-0c623066013b?auto=format&fit=crop&w=1400&q=85", "Competition Gallery", "Highlights from the arena and the field."),
            ("Venue", "venue", SportsVenueImage, "City Sports Arena", "Kochi, Kerala."),
            ("Certificates", "certificates", "https://images.unsplash.com/photo-1589330694653-ded6df03f754?auto=format&fit=crop&w=1400&q=85", "Certificates", "Participant and achievement certificates for eligible competitions."),
        });
        AddNavigation(db, e, db.EventPages.Local.Where(x => x.EventId == e.Id).OrderBy(x => x.DisplayOrder).ToArray());
    }

    private static void AddConferencePeople(EventCoreDbContext db, EventEntity e)
    {
        var speakers = new[]
        {
            new Speaker(e.Id, "Anika Menon", "Product and engineering leader focused on resilient digital platforms.", "VP Engineering", "Northstar Systems", "anika@example.com", "https://images.unsplash.com/photo-1551836022-d5d88e9218df?auto=format&fit=crop&w=700&q=85", 1),
            new Speaker(e.Id, "Rohan Iyer", "Architect working across distributed systems, APIs and event-driven platforms.", "Principal Architect", "Arcwell", "rohan@example.com", "https://images.unsplash.com/photo-1560250097-0b93528c311a?auto=format&fit=crop&w=700&q=85", 2),
            new Speaker(e.Id, "Maya Thomas", "Design strategist helping teams turn complex workflows into clear experiences.", "Design Director", "Studio North", "maya@example.com", "https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?auto=format&fit=crop&w=700&q=85", 3),
            new Speaker(e.Id, "Arjun Nair", "Platform engineer exploring messaging, observability and production reliability.", "Staff Platform Engineer", "Cloudline", "arjun@example.com", "https://images.unsplash.com/photo-1500648767791-00dcc994a43e?auto=format&fit=crop&w=700&q=85", 4)
        };
        db.Speakers.AddRange(speakers);
        var sessions = db.Sessions.Local.Where(x => x.EventId == e.Id).OrderBy(x => x.StartTimeUtc).ToArray();
        foreach (var pair in sessions.Zip(speakers, (session, speaker) => new { session, speaker }))
            db.SessionSpeakers.Add(new SessionSpeaker(pair.session.Id, pair.speaker.Id));

        db.Sponsors.AddRange(
            new Sponsor(e.Id, "Northstar Systems", "Technology partner supporting the summit.", "https://example.com", "https://images.unsplash.com/photo-1556761175-4b46a572b786?auto=format&fit=crop&w=800&q=85", "Platinum", 1),
            new Sponsor(e.Id, "Cloudline", "Infrastructure and platform partner.", "https://example.com", "https://images.unsplash.com/photo-1556761175-b413da4baf72?auto=format&fit=crop&w=800&q=85", "Gold", 2),
            new Sponsor(e.Id, "Studio North", "Design and experience partner.", "https://example.com", "https://images.unsplash.com/photo-1497366754035-f200968a6e72?auto=format&fit=crop&w=800&q=85", "Silver", 3));
    }

    private static void AddFestivalSponsors(EventCoreDbContext db, EventEntity e)
    {
        db.Sponsors.AddRange(
            new Sponsor(e.Id, "Malabar Foods", "Celebrating local food culture and community.", "https://example.com", "https://images.unsplash.com/photo-1515003197210-e0cd71810b5f?auto=format&fit=crop&w=800&q=85", "Presenting Partner", 1),
            new Sponsor(e.Id, "Heritage Collective", "Supporting arts, craft and cultural preservation.", "https://example.com", "https://images.unsplash.com/photo-1528698827591-e19ccd7bc23d?auto=format&fit=crop&w=800&q=85", "Cultural Partner", 2),
            new Sponsor(e.Id, "Coastline Bank", "Community partner for the festival experience.", "https://example.com", "https://images.unsplash.com/photo-1556761175-4b46a572b786?auto=format&fit=crop&w=800&q=85", "Community Partner", 3));
    }

    private static void AddSession(EventCoreDbContext db, EventEntity e, Section section, string title, string type, int year, int month, int day, int hourUtc, int minute, int endHourUtc, int endMinute, Venue venue, string imageUrl)
    {
        var start = new DateTime(year, month, day, hourUtc, minute, 0, DateTimeKind.Utc);
        var end = new DateTime(year, month, day, endHourUtc, endMinute, 0, DateTimeKind.Utc);
        db.Sessions.Add(new Session(e.Id, section.Id, title, $"{title} — a curated {type.ToLowerInvariant()} in the {e.Name} program.", type, venue.Capacity / 2, start, end, venue.Id, imageUrl));
    }

    private static EventPage AddPage(EventCoreDbContext db, EventEntity e, string name, string slug, string type, int order)
    {
        var page = new EventPage(e.Id, name, slug, type, order);
        page.Publish();
        db.EventPages.Add(page);
        return page;
    }

    private static void AddPageSet(EventCoreDbContext db, EventEntity e, IEnumerable<(string Name, string Slug, string Image, string Title, string Content)> pages)
    {
        var order = 1;
        foreach (var item in pages)
        {
            var page = AddPage(db, e, item.Name, item.Slug, item.Name, order++);
            AddSections(db, page, new[] { ("hero", item.Title, item.Content, item.Image) });
        }
    }

    private static void AddSections(EventCoreDbContext db, EventPage page, IEnumerable<(string Type, string Title, string Content, string? Image)> sections)
    {
        var order = 1;
        foreach (var section in sections)
        {
            db.PageSections.Add(new PageSection(page.Id, section.Type, section.Title, section.Content, section.Image, null, order++, null));
        }
    }

    private static void AddNavigation(EventCoreDbContext db, EventEntity e, params EventPage[] pages)
    {
        var menu = new NavigationMenu(e.Id, "Main Menu", "Header");
        db.NavigationMenus.Add(menu);
        var order = 1;
        foreach (var page in pages.OrderBy(x => x.DisplayOrder))
        {
            db.NavigationItems.Add(new NavigationItem(menu.Id, page.Name, null, page.Id, order++, false));
        }
    }
}
