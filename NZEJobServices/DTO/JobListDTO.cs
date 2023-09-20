using System.ComponentModel.DataAnnotations;
using NZEJobServices.Types;

/// <summary>
/// Job DTO to represent the aspects of the data to return to the user.
/// </summary>
/// <remarks>
/// It is considered best practice to not expose the data context object directly and only surface the relevant
/// properties for a given task
/// </remarks>
public class JobListDTO
{
    /// <summary>
    /// Unique job identifier
    /// </summary>
    public Guid ID { get; set; }

    /// <summary>
    /// Name of job
    /// </summary>
    [Required] // data annotation of required will specify in the output schema tha a field cannot be null
    public string Title { get; set; }

    /// <summary>
    /// Job general description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Current job stage
    /// </summary>
    public JobStage Stage { get; set; }

    /// <summary>
    /// Creates a new instance of <see cref="JobListDTO"/> with required values
    /// </summary>
    /// <param name="id">unique identifier for job</param>
    /// <param name="title">short form title</param>
    /// <param name="description">job details description, can be null</param>
    /// <param name="stage">current stage</param>
    public JobListDTO(Guid id, string title, string description, JobStage stage)
    {
        ID = id;
        Title = title;
        Description = description;
        Stage = stage;
    }

    /// <summary>
    /// Helper method to construct a DTO from the base data type
    /// </summary>
    /// <param name="job">job to convert to a DTO</param>
    /// <returns></returns>
    public static JobListDTO FromJob(Job job) => new JobListDTO(job.ID, job.Title, job.Description, job.Stage);
}