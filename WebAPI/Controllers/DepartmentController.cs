using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Data;
using WebAPI.Models;
using Newtonsoft.Json;
using System.IO;
using WebAPI.Helpers;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public DepartmentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public JsonResult Get()
        {
            
            DataTable table = new DataTable();

            /* Below code is for SQL DB purpose uncomment and use this one
            tring query = @"
             select DepartmentId, DepartmentName from dbo.Department";
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            SqlDataReader myReader;
            using(SqlConnection myCon=new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader); ;

                    myReader.Close();
                    myCon.Close();
                }
            }
            */
            using (StreamReader r = new StreamReader(Path.Combine(Environment.CurrentDirectory, @"MocDB", "department.json")))
            {
                string json = r.ReadToEnd();
                table = (DataTable)JsonConvert.DeserializeObject(json, (typeof(DataTable)));
            }

            return new JsonResult(table);
        }


        [HttpPost]
        public JsonResult Post(Department dep)
        {
            //string query = @"
            //        insert into dbo.Department values 
            //        ('"+dep.DepartmentName+@"')
            //        ";
            //DataTable table = new DataTable();
            //string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            //SqlDataReader myReader;
            //using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            //{
            //    myCon.Open();
            //    using (SqlCommand myCommand = new SqlCommand(query, myCon))
            //    {
            //        myReader = myCommand.ExecuteReader();
            //        table.Load(myReader); ;

            //        myReader.Close();
            //        myCon.Close();
            //    }
            //}
            if (dep.DepartmentId != 0)
            {
                List<Department> departments = new List<Department>();
                JSONReadWrite readWrite = new JSONReadWrite();
                departments = JsonConvert.DeserializeObject<List<Department>>(readWrite.Read("department.json", "MocDB"));

                Department department = departments.FirstOrDefault(x => x.DepartmentName.Equals(dep.DepartmentName));
                if (department == null)
                {
                    departments.Add(dep);
                }
                else
                {
                    //int index = departments.FindIndex(x => x.DepartmentId== dep.DepartmentId);
                    //departments[index] = department;
                    return new JsonResult("Department Already Exists!!! Pick a different Department!!!");
                }
                string jSONString = JsonConvert.SerializeObject(departments);
                readWrite.Write("department.json", "MocDB", jSONString);

                return new JsonResult("Added Successfully");
            }
            else
            {
                return new JsonResult("Department Id can't be 0 or empty!!!");
            }
        }


        [HttpPut]
        public JsonResult Put(Department dep)
        {
            string query = @"
                    update dbo.Department set 
                    DepartmentName = '"+dep.DepartmentName+@"'
                    where DepartmentId = "+dep.DepartmentId + @" 
                    ";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader); ;

                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult("Updated Successfully");
        }


        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            string query = @"
                    delete from dbo.Department
                    where DepartmentId = " + id + @" 
                    ";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader); ;

                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult("Deleted Successfully");
        }
    }
}
