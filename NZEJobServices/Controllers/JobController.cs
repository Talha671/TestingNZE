using Microsoft.AspNetCore.Mvc;
using NZEJobServices.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace NZEJobServices.Controllers;

/// <summary>
/// job functions controller
/// </summary>
/// <remarks>
/// <list type="bullet">
/// <item>ApiController attribute signals the class is the be incorporated into the API definition.</item>
/// <item>Route determines the base path that represents, the `[controller]` syntax indicates it should use the
/// class name prior to the Controller component</item>
/// </list>
/// </remarks>
[ApiController]
[Route("[controller]")]
 [Authorize(Policy = "AuthZPolicy")]
public class JobsController : ControllerBase
{
    /// <summary>
    /// Event identifiers for logging output
    /// </summary>
    /// <remarks>
    /// Microsoft recommends specifying ids for each of your logical even groupings to use when processing log output.
    ///
    /// General format of the output will be `Full.Class.Name [EventID]`
    /// </remarks>
    static class EventIds
    {
        public static int Index = 1000;
        public static int Create = 2000;
        public static int Update = 3000;
        public static int Delete = 4000;
        public static int List = 5000;
        public static int Sample = 6000;
    }

    private readonly ILogger<JobsController> _logger;
    private readonly JobContext _context;


    /// <summary>
    /// Job controller class
    /// </summary>
    /// <param name="logger">default logger instance for dependency injection</param>
    /// <param name="context">default context, see Program.cs for how to add injected instance</param>
    public JobsController(ILogger<JobsController> logger, JobContext context)
    {
        _logger = logger;
        _context = context;
    }


    /// <summary>
    /// Returns the current list of jobs
    /// </summary>
    /// <param name="stage">job results filter</param>
    /// <returns></returns>
    [HttpGet("list")] // specifies the accessible path, appended to any top level Route value
    [Produces("application/json")] // specifies the Content-Type for endpoint
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    public async Task<ActionResult<IEnumerable<Job>>> List(JobStage? stage = null)
    {
        try
        {
            return Ok(1);

        }
        catch (Exception)
        {
            var error = new Error(StatusCodes.Status400BadRequest, "Unable to retrieve job list", "JOB_LIST", "JL-2");
            _logger.LogError(EventIds.List, null, error.Message);
            return BadRequest(error);
        }
    }

    /// <summary>
    /// Retrieves a summary view of the jobs list
    /// </summary>
    /// <param name="stage">job results filter</param>
    /// <remarks>
    /// In practice the data context level type should not be directly returned through to the front end. This example
    /// produces a summary jobs list where only the ID, title, description and stage are displayed
    /// </remarks>
    /// <returns></returns>
    [Authorize(Policy = "AuthZPolicy")]
    // [Authorize]
    [HttpGet("summary")] // specifies the accessible path, appended to any top level Route value
    [Produces("application/json")] // specifies the Content-Type for endpoint
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    public async Task<ActionResult<IEnumerable<JobListDTO>>> Summary(JobStage? stage = null)
    {
        try
        {
            var results = (await _context.GetJobs(stage)).Select(JobListDTO.FromJob);

            // convert the selected jobs to a set of DTO objects before returning
            return Ok(results);

        }
        catch (Exception)
        {
            var error = new Error(StatusCodes.Status400BadRequest, "Unable to retrieve job list", "JOB_LIST", "JL-2");
            _logger.LogError(EventIds.List, null, error.Message);
            return BadRequest(error);
        }
    }

    /// <summary>
    /// Fills the API with some sample job data
    /// </summary>
    /// <param name="count">number of sample jobs to generate, default is 5</param>
    /// <returns></returns>
    [HttpGet("sample", Name = "Sample")] // specifies the accessible path, appended to any top level Route value
    [Produces("application/json")] // specifies the Content-Type for endpoint
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    public async Task<ActionResult<IEnumerable<Job>>> CreateSampleData(int count = 5)
    {
        try
        {
            var current = await _context.GetJobs();
            _context.Jobs.RemoveRange(current);

            for (int i = 0; i < count; i++)
            {
                _context.Jobs.Add(Job.NewSample());
            }

            await _context.SaveChangesAsync();

            return Ok(await _context.Jobs.ToListAsync());

        }
        catch (Exception ex)
        {
            _logger.LogError(37, ex, "error");
            var error = new Error(StatusCodes.Status400BadRequest, "Could not generate samples", "SAMPLE_CREATE", "JS-1");
            _logger.LogError(EventIds.Sample, error.Message);
            return BadRequest(error);
        }
    }

    /// <summary>
    /// Attempts to retrieve a given job by id
    /// </summary>
    /// <param name="id">unique job id</param>
    /// <returns></returns>
    [HttpGet("{id}", Name = "Get")]
    [Produces("application/json")] // specifies the Content-Type for endpoint
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Job>> Get(Guid id)
    {
        try
        {
            var found = await _context.GetJob(id);

            if (found == null)
            {
                _logger.LogWarning(EventIds.Index, $"Job not found {id}");

                return NotFound();
            }

            return Ok(found);
        }
        catch (Exception)
        {
            var error = new Error(StatusCodes.Status400BadRequest, $"Unable to retrieve job {id}", "JOB_GET", "JG-2");
            _logger.LogError(EventIds.Index, error.Message);
            return BadRequest(error);
        }
    }

    /// <summary>
    /// Updates the values of a job
    /// </summary>
    /// <param name="job"></param>
    /// <returns></returns>
    [HttpPut(Name = "Put")]
    [Produces("application/json")] // specifies the Content-Type for endpoint
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Job>> Put(Job job)
    {
        try
        {
            var existing = await _context.GetJob(job.ID);

            if (existing == null)
            {
                return NotFound();
            }

            // replace existing with new details
            await _context.UpdateJob(job);

            return Ok(job);
        }
        catch (Exception)
        {
            var error = new Error(StatusCodes.Status400BadRequest, $"Unable to update job {job.ID}", "JOB_UPDATE", "JU-1");
            _logger.LogError(EventIds.Update, error.Message);
            return BadRequest(error);
        }
    }

    /// <summary>
    /// Deletes a job with the given ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}", Name = "Delete")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            var existing = await _context.GetJob(id);

            if (existing == null)
            {
                return NotFound();
            }

            await _context.DeleteJob(existing);

            return NoContent();
        }
        catch (Exception)
        {
            var error = new Error(StatusCodes.Status400BadRequest, $"Unable to delete job {id}", "JOB_DELETE", "JD-1");
            _logger.LogError(EventIds.Update, error.Message);
            return BadRequest(error);
        }
    }

    /// <summary>
    /// Purposefully throw an error
    /// </summary>
    /// <returns></returns>
    [HttpGet("error", Name = "Error")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    public ActionResult<Error> Error()
    {
        var error = new Error(StatusCodes.Status400BadRequest, "forced error thrown", "JOB_ERROR", "JE-1");
        _logger.LogError(37, error.Message);

        return BadRequest(error);
    }
}
