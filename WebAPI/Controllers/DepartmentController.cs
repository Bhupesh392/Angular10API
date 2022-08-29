using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
            if (dep.DepartmentId != 0 && !string.IsNullOrEmpty(dep.DepartmentName))
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
                return new JsonResult("Department can't be null or empty!!");
            }
        }


        [HttpPut]
        public JsonResult Put(Department dep)
        {
            if (dep.DepartmentId != 0 && !string.IsNullOrEmpty(dep.DepartmentName))
            {
                List<Department> departments = new List<Department>();
                JSONReadWrite readWrite = new JSONReadWrite();
                departments = JsonConvert.DeserializeObject<List<Department>>(readWrite.Read("department.json", "MocDB"));

                departments.FirstOrDefault(x => x.DepartmentId == dep.DepartmentId).DepartmentName = dep.DepartmentName;
                
                string jSONString = JsonConvert.SerializeObject(departments);
                readWrite.Write("department.json", "MocDB", jSONString);

                return new JsonResult("Updated Successfully");
            }

            return new JsonResult("Not Updated!!");
        }


        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            List<Department> departments = new List<Department>();
            JSONReadWrite readWrite = new JSONReadWrite();
            departments = JsonConvert.DeserializeObject<List<Department>>(readWrite.Read("department.json", "MocDB"));

            departments.RemoveAt(departments.FindIndex(x => x.DepartmentId == id));

            string jSONString = JsonConvert.SerializeObject(departments);
            readWrite.Write("department.json", "MocDB", jSONString);

            return new JsonResult("Deleted Successfully");
        }
    }
}
