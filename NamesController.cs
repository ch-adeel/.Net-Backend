using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks; // Needed for asynchronous programming
using System.Web.Http;
using BackendApp.Models; 
using Amazon.DynamoDBv2; // DynamoDB client namespace
using Amazon.DynamoDBv2.DataModel; // DynamoDB data model namespace
using Amazon.DynamoDBv2.DocumentModel; // DynamoDB document model namespace

namespace BackendApp.Controllers
{
    public class NamesController : ApiController
    {
        // Define the DynamoDB client and context
        private static AmazonDynamoDBClient client;
        private static DynamoDBContext context;

        // Constructor to initialize the client and context
        public NamesController()
        {
            // Initialize the DynamoDB client with local settings
            AmazonDynamoDBConfig config = new AmazonDynamoDBConfig
            {
                ServiceURL = "http://localhost:8000" // Local DynamoDB endpoint
            };

            client = new AmazonDynamoDBClient(config); // Create the client
            context = new DynamoDBContext(client); // Create the context for data operations
        }

        // GET api/names
        [HttpGet]
        public async Task<IHttpActionResult> Get() // Use async for non-blocking operations
        {
            try
            {
                // Scan the table to get all names
                var search = context.ScanAsync<NameModel>(new List<ScanCondition>());
                var names = await search.GetRemainingAsync(); // Retrieve all items asynchronously

                // Return the list of names from DynamoDB
                return Ok(names.Select(n => n.Name));
            }
            catch (Exception ex)
            {
                // Handle any errors that occurred during the operation
                return InternalServerError(ex);
            }
        }

        // POST api/names
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] NameModel nameModel)
        {
            if (nameModel == null || string.IsNullOrEmpty(nameModel.Name))
            {
                return BadRequest("Name cannot be empty");
            }

            try
            {
                // Add the new name to the DynamoDB table
                await context.SaveAsync(nameModel);

                // Return a 201 Created status with the newly added name
                return Created($"api/names/{nameModel.Name}", nameModel.Name);
            }
            catch (Exception ex)
            {
                // Handle any errors that occurred during the operation
                return InternalServerError(ex);
            }
        }
    }

    // Define the NameModel with DynamoDB attributes
    [DynamoDBTable("LoginForm-Dev")] // Specify the DynamoDB table name
    public class NameModel
    {
        [DynamoDBHashKey] // Indicates that this is the primary key
        public string Name { get; set; }
    }
}
