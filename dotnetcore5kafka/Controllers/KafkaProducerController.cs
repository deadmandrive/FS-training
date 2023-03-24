using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace dotnetcore5kafka.Controllers
{
    [Route("api/kafka")]
    [ApiController]
    public class KafkaProducerController : ControllerBase
    {
        private ProducerConfig producerConfig;
        private SchemaRegistryConfig schemaregistryConfig;


        private readonly ProducerConfig config = new ProducerConfig
        { BootstrapServers = "localhost:9092" };
        private readonly string topic = "simpletalk_topic";


        public KafkaProducerController(ProducerConfig producerConfig,SchemaRegistryConfig schemaregistryConfig)
        {
            this.producerConfig = producerConfig;
            this.schemaregistryConfig = schemaregistryConfig;
            this.producerConfig.SaslMechanism = SaslMechanism.ScramSha512;


        }

        [HttpPost("produce")]
        public async Task<ActionResult> ProduceMessage(string topic, [FromBody] ApplicationLog message)
        {
            string serailizedTestData = JsonConvert.SerializeObject(message);
            // Note: Specifying json serializer configuration is optional.
            var jsonSerializerConfig = new JsonSerializerConfig
            {
                BufferBytes = 100
            };

            using (var schemaRegistry = new CachedSchemaRegistryClient(schemaregistryConfig))
            {
                //var schema = await schemaRegistry.GetLatestSchemaAsync(SubjectNameStrategy.Topic.ConstructValueSubjectName(topic));
                
                using (var producer = new ProducerBuilder<string, ApplicationLog>(producerConfig)
                    .SetValueSerializer(new JsonSerializer<ApplicationLog>(schemaRegistry, jsonSerializerConfig)).Build())
                {
                   
                    await producer.ProduceAsync(topic, new Message<string, ApplicationLog> { Value = message });
                    producer.Flush(TimeSpan.FromSeconds(10));


                }
            }

            //using (var schemaRegistry = new CachedSchemaRegistryClient(schemaregistryConfig))
            //{
            //    // Note: a subject name strategy was not configured, so the default "Topic" was used.
            //    var schema = await schemaRegistry.GetLatestSchemaAsync(SubjectNameStrategy.Topic.ConstructValueSubjectName(topic));
               
            //    Console.WriteLine("\nThe JSON schema corresponding to the written data:");
            //    Console.WriteLine(schema.SchemaString);
            //}
            return Ok(true);
        }

       

    }

    public class TestData
    {
        [JsonRequired]
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonRequired]
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonRequired]
        [JsonProperty("address")]
        public string address { get; set; }
        [JsonRequired]
        [JsonProperty("tel")]
        public string tel { get; set; }
        [JsonProperty("isadmin")]
        public bool isAdmin { get; set; }
    }

    public class ApplicationLog
    {
        public DateTime TimeStamp { get; set; }
        public string? LogLevel { get; set; }
        public string? LogSystem { get; set; }
        public string? User { get; set; }
        public string? Module { get; set; }
        public string? SubModule { get; set; }
        public string? HttpStatusCode { get; set; }
        public string? HttpMethod { get; set; }
        public string? LogMessage { get; set; }

    }
}

