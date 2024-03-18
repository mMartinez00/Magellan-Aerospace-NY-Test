using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace MagellanTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemsController : ControllerBase
    {
        private  static string? EnvUser;
        private  static string? DbPassword;
        private static string? connString;
        public ItemsController(){
            EnvUser = Environment.GetEnvironmentVariable("User");
            DbPassword = Environment.GetEnvironmentVariable("DbPassword");
            connString = $"Host=localhost;Port=5432;Database=Part;Username={EnvUser};Password={DbPassword}";

        }

        [HttpPost]
        public IActionResult PostNewRecord(Record record)
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connString))
                {
                    conn.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand("INSERT INTO item (item_name, parent_item, cost, req_date) VALUES (@ItemName, @ParentItem, @Cost, @ReqDate) RETURNING id;", conn))

                    {
                        if (record.ParentItem != null)
                        {
                            command.Parameters.AddWithValue("@ParentItem", record.ParentItem);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@ParentItem", DBNull.Value);
                        }

                        command.Parameters.AddWithValue("@ItemName", record.ItemName); ;
                        command.Parameters.AddWithValue("@Cost", record.Cost);
                        command.Parameters.AddWithValue("@ReqDate", record.ReqDate);

                        var id = command.ExecuteScalar();

                        return Ok(id);
                    }


                }
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }

        }

        [HttpGet("{id:int}")]
        public IActionResult GetRecord(int id)
        {
            Record record = new Record();

            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connString))
                {
                    conn.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand("SELECT id, item_name, parent_item, cost, req_date FROM item WHERE id = @Id;", conn))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                record.ID = reader.GetInt32(0);
                                record.ItemName = reader.GetString(1);
                                record.ParentItem = reader.IsDBNull(2) ? (int?)null : reader.GetInt32(2);
                                record.Cost = reader.GetInt32(3);
                                record.ReqDate = reader.GetDateTime(4);
                            }
                            else
                            {
                                return NotFound();
                            }
                        }
                    }
                }

                return Ok(record);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }


        }

        [HttpGet("GetTotalCost/{item_name}")]
        public IActionResult GetTotalCost(string item_name)
        {

            Get_Total_Cost cost = new Get_Total_Cost();

            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connString))
                {
                    conn.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand("SELECT Get_Total_Cost(@item_name);", conn))
                    {
                        command.Parameters.AddWithValue("@item_name", item_name);

                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                cost.Total_Cost = reader.GetInt32(0);
                            }

                        }
                    }
                }

                return Ok(cost);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }



        }
    }

    public class Get_Total_Cost
    {
        public int? Total_Cost { get; set; }
    }
    public class Record
    {
        public int ID { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int? ParentItem { get; set; }
        public int Cost { get; set; }
        public DateTime ReqDate { get; set; }
    }

}

