using MIS704.Domain;
namespace MIS704.Application;
public sealed class EnrollStudentHandler
{
    private readonly IEnrollmentRepository _repository;
    private readonly IEventPublisher _events;
    public EnrollStudentHandler(IEnrollmentRepository repository, IEventPublisher events)
    { _repository = repository; _events = events; }
    public async Task<Enrollment> Handle(EnrollStudentCommand command, CancellationToken ct = default)
    {
        var enrollment = new Enrollment(command.StudentId, command.CourseId, command.Credits);
        await _repository.AddAsync(enrollment, ct);
        await _events.PublishAsync(new EnrollmentCreated(enrollment.Id, enrollment.StudentId, enrollment.CourseId), ct);
        return enrollment;
    }
}
public sealed class GetEnrollmentHandler
{
    private readonly IEnrollmentRepository _repository;
    public GetEnrollmentHandler(IEnrollmentRepository repository) => _repository = repository;
    public Task<Enrollment?> Handle(GetEnrollmentQuery query, CancellationToken ct = default)
        => _repository.FindAsync(query.EnrollmentId, ct);
}
