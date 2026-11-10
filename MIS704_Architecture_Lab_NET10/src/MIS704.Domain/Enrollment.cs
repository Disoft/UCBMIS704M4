namespace MIS704.Domain;
public sealed class Enrollment
{
    public Guid Id { get; }
    public Guid StudentId { get; }
    public Guid CourseId { get; }
    public int Credits { get; }
    public Enrollment(Guid studentId, Guid courseId, int credits)
    {
        if (studentId == Guid.Empty || courseId == Guid.Empty) throw new ArgumentException("Identificadores requeridos");
        if (credits is < 1 or > 8) throw new ArgumentOutOfRangeException(nameof(credits));
        Id = Guid.NewGuid(); StudentId = studentId; CourseId = courseId; Credits = credits;
    }
}
public static class TuitionCalculator
{
    public static decimal Calculate(int credits, decimal pricePerCredit)
    {
        if (credits < 0 || pricePerCredit < 0) throw new ArgumentOutOfRangeException(nameof(credits));
        return credits * pricePerCredit;
    }
}
