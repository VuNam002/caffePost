namespace CaffePOS.Model.Interfaces
{
    public interface ITimestamped
    {
        DateTime? created_at { get; set; }
        DateTime? updated_at { get; set; }
    }
}