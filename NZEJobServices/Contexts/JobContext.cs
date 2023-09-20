
using Microsoft.EntityFrameworkCore;
using NZEJobServices.Types;

/// <summary>
/// Main context for the Job data type
/// </summary>
/// <remarks>
/// A context should represent a set of related data, which may map to a table or multiple related tables in the
/// underlying data source dependant on the data relationships and underlying capabilities
/// </remarks>
public class JobContext : DbContext
{
    /// <summary>
    /// Defines a DbSet of the Job data type. If this type were to contain underlying related objects (e.g. a list of
    /// associated users) Then a DbSet of Users should be defined to represent the users type. The Context will then
    /// construct the underlying representations and relational keys to tie the types together
    /// </summary>
    public DbSet<Job> Jobs { get; set; }

    /// <summary>
    /// Controllers taking an options context should provide a constructor that passes the options to the base class
    /// constructor in order to correctly consume its configuration
    /// </summary>
    /// <param name="options">The options for this context.</param>
#pragma warning disable CS8618
    public JobContext(DbContextOptions options) : base(options)
    {

    }
#pragma warning restore CS8618

    /// <summary>
    /// Handle job retrieval and (future) pagination
    /// </summary>
    /// <param name="stage">optional, stage to filter jobs by</param>
    /// <param name="start">optional, starting position to retrieve jobs from</param>
    /// <param name="count">optional, number of results to take</param>
    /// <returns></returns>
    public async Task<ICollection<Job>> GetJobs(JobStage? stage = null, int start = 0, int count = 0) {
        var set = (stage != null) ? Jobs.Where(j => j.Stage == stage) : Jobs;

        // NOTE: this is for example purposes, documentation recommends performing keyset pagination where possible
        // see https://learn.microsoft.com/en-us/ef/core/querying/pagination
        set = set.Skip(start);

        if (count != 0) {
            set = set.Take(count);
        }

        return await set.ToListAsync();
    }

    /// <summary>
    /// Searches for a Job based on its ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<Job?> GetJob(Guid id) {
        return await Jobs.FirstOrDefaultAsync(job => job.ID == id);
    }

    /// <summary>
    /// Removes a job from the set and saves changes to the context
    /// </summary>
    /// <param name="job"></param>
    /// <returns></returns>
    public async Task DeleteJob(Job job) {
        Jobs.Remove(job);

        await SaveChangesAsync();
    }

    /// <summary>
    /// Replaces a job
    /// </summary>
    /// <param name="job"></param>
    /// <returns></returns>
    public async Task UpdateJob(Job job) {
        Jobs.Remove(job);
        Jobs.Add(job);

        await SaveChangesAsync();
    }
}