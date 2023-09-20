using System.ComponentModel.DataAnnotations;

namespace NZEJobServices.Types;

/// <summary>
/// Represents a generic error response
/// </summary>
public class Error
{
    /// <summary>
    /// status code from <seealso cref="StatusCodes"/>
    /// </summary>
    [Required]
    public int Code { get; set; }

    /// <summary>
    /// Human readable description of error
    /// </summary>
    [Required]
    public string Message { get; set; }

    /// <summary>
    /// Machine processable status description format is "EXAMPLE_ERROR_STATUS"
    /// </summary>
    [Required]
    public string Status { get; set; }

    /// <summary>
    /// additional processing details to use to determine what to do with the response
    /// </summary>
    [Required]
    public ErrorDetails Details { get; set; }

    /// <summary>
    /// Create a new Error
    /// </summary>
    /// <param name="code">error code, should match http response code returned, <seealso cref="StatusCodes"/></param>
    /// <param name="message">user processable message</param>
    /// <param name="status">machine processable status</param>
    /// <param name="reason">log matchable reason for error</param>
    /// <param name="domain">problem source, default <seealso cref="Domain.APP"/> </param>
    /// <param name="metadata">Optional, accompanying metadata</param>
    public Error(int code, string message, string status, string reason, Domain domain = Domain.APP, object? metadata = null)
    {
        Code = code;
        Message = message;
        Status = status;
        Details = new ErrorDetails(reason, domain, metadata);
    }

}

/// <summary>
/// Detailed breakdown of provided error
/// </summary>
public class ErrorDetails
{
    /// <summary>
    /// traceable value to provide
    /// </summary>
    [Required]
    public string Reason { get; set; }

    /// <summary>
    /// Domain of encountered error, identifies problem service
    /// </summary>
    [Required]
    public Domain Domain { get; set; }

    /// <summary>
    /// Optional, additional data to assist in processing the issue
    /// </summary>
    public object? Metadata { get; set; }

    /// <summary>
    ///
    /// </summary>
    /// <param name="reason"></param>
    /// <param name="domain"></param>
    /// <param name="metadata"></param>
    public ErrorDetails(string reason, Domain domain = Domain.APP, object? metadata = null)
    {
        Reason = reason;
        Domain = domain;
        Metadata = metadata;
    }
}



/// <summary>
/// Available domains
/// </summary>
public enum Domain
{
    /// <summary>
    /// represents app service
    /// </summary>
    APP,
    /// <summary>
    /// represents backing storage service
    /// </summary>
    STORAGE,
    /// <summary>
    /// represents backing function process
    /// </summary>
    TENANT
}
