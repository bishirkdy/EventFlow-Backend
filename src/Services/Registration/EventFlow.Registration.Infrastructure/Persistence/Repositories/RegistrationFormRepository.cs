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
        db.RegistrationFormFields.RemoveRange(
            db.RegistrationFormFields
                .Where(x => x.RegistrationFormId == form.Id));

        foreach (var field in fields)
        {
            db.RegistrationFormFields.Add(field);
        }
    }
}
