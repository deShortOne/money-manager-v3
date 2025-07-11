
using System.Text.Json.Serialization;

namespace MoneyTracker.Commands.Infrastructure.AWS;

public class MessageBody
{
    public List<Record> Records { get; set; } = new List<Record>();
}

public class Record
{
    [JsonPropertyName("awsRegion")]
    public string AWSRegion { get; set; } = string.Empty;


    [JsonPropertyName("eventName")]
    public string EventName { get; set; } = string.Empty;


    [JsonPropertyName("eventSource")]
    public string EventSource { get; set; } = string.Empty;


    [JsonPropertyName("eventTime")]
    public DateTime EventTime { get; set; }


    [JsonPropertyName("eventVersion")]
    public string EventVersion { get; set; } = string.Empty;


    [JsonPropertyName("requestParameters")]
    public Requestparameters RequestParameters { get; set; } = new Requestparameters();


    [JsonPropertyName("responseElements")]
    public ResponseElements ResponseElements { get; set; } = new ResponseElements();


    [JsonPropertyName("s3")]
    public S3 S3 { get; set; }

    [JsonPropertyName("userIdentity")]
    public Useridentity UserIdentity { get; set; } = new Useridentity();
}

public class Requestparameters
{
    [JsonPropertyName("sourceIPAddress")]
    public string SourceIPAddress { get; set; } = string.Empty;
}

public class ResponseElements
{
    [JsonPropertyName("xamzid2")]
    public string Xamzid2 { get; set; } = string.Empty;

    [JsonPropertyName("xamzrequestid")]
    public string Xamzrequestid { get; set; } = string.Empty;
}

public class S3
{
    [JsonPropertyName("bucket")]
    public Bucket Bucket { get; set; } = new Bucket();

    [JsonPropertyName("configurationId")]
    public string ConfigurationId { get; set; } = string.Empty;

    [JsonPropertyName("object")]
    public S3Object S3Object { get; set; } = new S3Object();

    [JsonPropertyName("s3SchemaVersion")]
    public string S3SchemaVersion { get; set; } = string.Empty;
}

public class Bucket
{
    [JsonPropertyName("arn")]
    public string Arn { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("ownerIdentity")]
    public Owneridentity OwnerIdentity { get; set; } = new Owneridentity();
}

public class Owneridentity
{
    [JsonPropertyName("principalId")]
    public string PrincipalId { get; set; } = string.Empty;
}

public class S3Object
{
    [JsonPropertyName("eTag")]
    public string ETag { get; set; } = string.Empty;

    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;

    [JsonPropertyName("sequencer")]
    public string Sequencer { get; set; } = string.Empty;

    [JsonPropertyName("size")]
    public int Size { get; set; } = -1;
}

public class Useridentity
{
    [JsonPropertyName("principalId")]
    public string PrincipalId { get; set; } = string.Empty;
}
