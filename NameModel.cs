using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BackendApp.Models
{
    [DynamoDBTable("LoginForm-Dev")] // This maps the class to your DynamoDB table
    public class NameModel
    {
        [DynamoDBHashKey] // Indicates that 'Name' is the primary key
        public string Name { get; set; }
    }
}