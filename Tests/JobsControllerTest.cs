using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NZEJobServices.Controllers;

namespace Tests;

public class JobsControllerTest
{
    private readonly ILogger<JobsController> _logger;

    private const int SAMPLE_SIZE = 10;

    public JobsControllerTest()
    {
        // simple indicator of using the mock framework to create an object of the right type
        _logger = new Mock<ILogger<JobsController>>().Object;
    }

    /// <summary>
    /// Initialise a new controller for a test
    /// </summary>
    /// <param name="name">unique name for in memory store, recommend using test method name</param>
    /// <returns></returns>
    private JobsController _GetController(string name) {
        var context = new JobContext(new DbContextOptionsBuilder<JobContext>().UseInMemoryDatabase(name).Options);
        return new JobsController(_logger, context);
    }

    [Fact]
    public async void GenerateSample1()
    {
        var controller = _GetController(nameof(GenerateSample1));
        var samples = GetObjectResultContent(await controller.CreateSampleData(SAMPLE_SIZE));

        Assert.NotNull(samples);
        Assert.Equal(SAMPLE_SIZE, samples?.Count());
    }

    [Fact]
    public async void List()
    {
        var controller = _GetController(nameof(List));
        var samples = GetObjectResultContent(await controller.CreateSampleData(SAMPLE_SIZE));

        // pull same results back as summary, number of jobs should still match
        var summary = GetObjectResultContent(await controller.List());
        Assert.NotNull(summary);
        Assert.Equal(SAMPLE_SIZE, summary!.Count());
    }

    [Fact]
    public async void Summary()
    {
        var controller = _GetController(nameof(Summary));
        var samples = GetObjectResultContent(await controller.CreateSampleData(SAMPLE_SIZE));

        // pull same results back as summary, number of jobs should still match
        var summary = GetObjectResultContent(await controller.Summary());
        Assert.NotNull(summary);
        Assert.Equal(SAMPLE_SIZE, summary?.Count());
    }

    [Fact]
    public async void ModifyJob()
    {
        var controller = _GetController(nameof(ModifyJob));
        var samples = GetObjectResultContent(await controller.CreateSampleData(SAMPLE_SIZE));
        Assert.NotNull(samples);

        // modify the job with the PUT request, then check the given value changed
        var sample = samples?.FirstOrDefault();
        Assert.NotNull(sample);

        var originalTest = sample!.Description;
        var newDescription = "We Changed The Description";
        sample.Description = newDescription;

        await controller.Put(sample);
        var changed = GetObjectResultContent(await controller.Get(sample.ID));

        Assert.Equal(changed?.Description, newDescription);
    }

    [Fact]
    public async void DeleteJob()
    {
        var controller = _GetController(nameof(DeleteJob));
        var samples = GetObjectResultContent(await controller.CreateSampleData(SAMPLE_SIZE));
        Assert.NotNull(samples);

        // modify the job with the PUT request, then check the given value changed
        var sample = samples?.FirstOrDefault();
        Assert.NotNull(sample);

        await controller.Delete(sample!.ID);

        // NOTE: getting to the NotFound state will cause our mock logger object to be called
        var notFound = await controller.Get(sample.ID);
        Assert.IsType<NotFoundResult>(notFound.Result);
    }

    /// <summary>
    /// Helper method to get typed internal result
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="result"></param>
    /// <returns></returns>
    /// <remarks>
    /// <see cref="ActionResult{T}"/> populates either Result or Value with Result being populated when working with
    /// the Ok() NotFound() methods, this is a shorthand structure for retrieving the internal object with the expected
    /// result type from an <see cref="ActionResult{T}"/>
    /// </remarks>
    public static T? GetObjectResultContent<T>(ActionResult<T> result) where T : class
    {
        return (result.Result as OkObjectResult)?.Value as T;
    }
}
