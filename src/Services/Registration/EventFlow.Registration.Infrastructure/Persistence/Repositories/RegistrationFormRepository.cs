using EventFlow.Registration.Application.Abstractions.Persistence;

namespace EventFlow.Registration.Infrastructure.Persistence.Repositories;

public sealed class RegistrationFormRepository(RegistrationDbContext db)
    : IRegistrationFormRepository
{
    public async Task<RegistrationForm?> GetByEventIdAsync(
        Guid eventId,
        bool includeFields = false,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default)
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
            x => x.EventId == eventId,
            cancellationToken);
    }

    public async Task<IReadOnlySet<Guid>> GetFieldIdsWithAnswersAsync(
        IReadOnlyCollection<Guid> fieldIds,
        CancellationToken cancellationToken = default)
    {
        if (fieldIds.Count == 0)
        {
            return new HashSet<Guid>();
        }

        var ids = await db.RegistrationAnswers
            .Where(x => fieldIds.Contains(x.RegistrationFormFieldId))
            .Select(x => x.RegistrationFormFieldId)
            .Distinct()
            .ToListAsync(cancellationToken);

        return ids.ToHashSet();
    }

    public void Add(RegistrationForm form)
    {
        db.RegistrationForms.Add(form);
    }

    public void ReplaceFields(
        RegistrationForm form,
        IEnumerable<RegistrationFormField> fields)
    {
        var incoming = fields.ToList();
        var existing = form.Fields.ToList();
        var incomingIds = incoming
            .Where(x => x.Id != Guid.Empty)
            .Select(x => x.Id)
            .ToHashSet();

        foreach (var existingField in existing)
        {
            if (incomingIds.Contains(existingField.Id))
            {
                continue;
            }

            db.RegistrationFormFields.Remove(existingField);
            form.Fields.Remove(existingField);
        }

        var existingById = existing.ToDictionary(x => x.Id);

        foreach (var field in incoming)
        {
            field.RegistrationFormId = form.Id;

            if (field.Id != Guid.Empty)
            {
                if (!existingById.TryGetValue(field.Id, out var existingField))
                {
                    throw new InvalidOperationException(
                        "The registration form contains a field that does not belong to this form.");
                }

                existingField.FieldKey = field.FieldKey;
                existingField.Label = field.Label;
                existingField.FieldType = field.FieldType;
                existingField.IsRequired = field.IsRequired;
                existingField.DisplayOrder = field.DisplayOrder;
                existingField.OptionsJson = field.OptionsJson;
                existingField.ValidationJson = field.ValidationJson;
                continue;
            }

            field.Id = Guid.NewGuid();
            form.Fields.Add(field);
            db.RegistrationFormFields.Add(field);
        }
    }
}
