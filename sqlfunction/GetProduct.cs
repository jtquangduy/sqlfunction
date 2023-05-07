using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using Azure;

namespace sqlfunction
{
    public static class GetProduct
    {
        [FunctionName("GetProduct")]
        public static async Task<IActionResult> RunProducts(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            List<Product> _products = new List<Product>();

            string _statement = "SELECT ProductID,ProductName,Quantity from Products";

            SqlConnection _conn = GetConnection();

            _conn.Open();

            SqlCommand cmd = new SqlCommand(_statement, _conn);

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Product product = new Product()
                    {
                        ProductId = reader.GetInt32(0),
                        ProductName = reader.GetString(1),
                        Quantity = reader.GetInt32(2)
                    };

                    _products.Add(product);
                }
            }
            _conn.Close();

            return new OkObjectResult(_products);
        }

        [FunctionName("GetProductById")]
        public static async Task<IActionResult> RunProduct(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            int productId = int.Parse(req.Query["id"]);

            string _statement = String.Format("SELECT ProductID,ProductName,Quantity from Products where ProductId={0}", productId);

            SqlConnection _conn = GetConnection();

            _conn.Open();

            SqlCommand cmd = new SqlCommand(_statement, _conn);

            Product product = new Product();

            try
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    product.ProductId = reader.GetInt32(0);
                    product.ProductName = reader.GetString(1);
                    product.Quantity = reader.GetInt32(2);
                    var response = product;
                _conn.Close();
                    return new OkObjectResult(response);
                }
            }
            catch (Exception ex)
            {
                var response = ex;
                _conn.Close();
                return new OkObjectResult(response);
            }
        }

        private static SqlConnection GetConnection()
        {
            string connectionString = "Server=tcp:jtqdappserver.database.windows.net,1433;Initial Catalog=appdb;Persist Security Info=False;User ID=sqladmin;Password=hoilamgi01@abcd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            return new SqlConnection(connectionString);
        }
    }
}
