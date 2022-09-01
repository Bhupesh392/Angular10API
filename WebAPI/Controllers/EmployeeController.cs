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
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using WebAPI.Helpers;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public EmployeeController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }
        public EmployeeController() { }

        [HttpGet]
        public JsonResult Get()
        {
            DataTable table = new DataTable();

            using (StreamReader r = new StreamReader(Path.Combine(Environment.CurrentDirectory, @"MocDB", "Employee.json")))
            {
                string json = r.ReadToEnd();
                table = (DataTable)JsonConvert.DeserializeObject(json, (typeof(DataTable)));
            }

            return new JsonResult(table);
        }

        [HttpPost]
        public IActionResult Post(Employee emp)
        {
            if (!string.IsNullOrEmpty(emp.EmployeeName))
            {
                emp.EmployeeId = new Random().Next();
                List<Employee> employees = new List<Employee>();
                JSONReadWrite readWrite = new JSONReadWrite();
                employees = JsonConvert.DeserializeObject<List<Employee>>(readWrite.Read("Employee.json", "MocDB"));

                Employee employee = employees.FirstOrDefault(x => x.EmployeeName.Equals(emp.EmployeeName));

                if (employee == null)
                {
                    employees.Add(emp);
                }
                else
                {
                    //int index = departments.FindIndex(x => x.DepartmentId== dep.DepartmentId);
                    //departments[index] = department;
                    return new JsonResult("Employee Already Exists!!! Pick a different Employee Name!!!");
                }
                string jSONString = JsonConvert.SerializeObject(employees);
                readWrite.Write("Employee.json", "MocDB", jSONString);

                return new JsonResult("Added Successfully");
            }
            var modelState = new ModelStateDictionary();
            modelState.AddModelError("EmployeeName", "Employee Name is required.");
            return BadRequest(modelState);

        }


        [HttpPut]
        public IActionResult Put(Employee emp)
        {
            if (emp.EmployeeId != 0 && !string.IsNullOrEmpty(emp.EmployeeName))
            {
                List<Employee> employees = new List<Employee>();
                JSONReadWrite readWrite = new JSONReadWrite();
                employees = JsonConvert.DeserializeObject<List<Employee>>(readWrite.Read("Employee.json", "MocDB"));

                Employee updatedEmp = employees.FirstOrDefault(x => x.EmployeeId == emp.EmployeeId);

                if (updatedEmp != null)
                {
                    updatedEmp.EmployeeName = emp.EmployeeName;
                    updatedEmp.Department = emp.Department;
                    updatedEmp.DateOfJoining = emp.DateOfJoining;
                    updatedEmp.PhotoFileName = emp.PhotoFileName;
                }

                string jSONString = JsonConvert.SerializeObject(employees);
                readWrite.Write("Employee.json", "MocDB", jSONString);

                return new JsonResult("Updated Successfully");
            }

            var modelState = new ModelStateDictionary();
            modelState.AddModelError("EmployeeName", "Employee Name is required.");
            return BadRequest(modelState);
        }


        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            List<Employee> employees = new List<Employee>();
            JSONReadWrite readWrite = new JSONReadWrite();
            employees = JsonConvert.DeserializeObject<List<Employee>>(readWrite.Read("Employee.json", "MocDB"));

            if (employees.FindIndex(x => x.EmployeeId == id) >= 0)
            {
                employees.RemoveAt(employees.FindIndex(x => x.EmployeeId == id));
            }
            else
            {
                var modelState = new ModelStateDictionary();
                modelState.AddModelError("DepartmentName", "Department Name is required.");
                return NotFound();
            }

            string jSONString = JsonConvert.SerializeObject(employees);
            readWrite.Write("Employee.json", "MocDB", jSONString);

            return new JsonResult("Deleted Successfully");
        }


        [Route("SaveFile")]
        [HttpPost]
        public JsonResult SaveFile()
        {
            try
            {
                var httpRequest = Request.Form;
                var postedFile = httpRequest.Files[0];
                string filename = postedFile.FileName;
                var physicalPath = _env.ContentRootPath + "/Photos/" + filename;

                using (var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }

                return new JsonResult(filename);
            }
            catch (Exception)
            {

                return new JsonResult("anonymous.png");
            }
        }


        [Route("GetAllDepartmentNames")]
        [HttpGet]
        public JsonResult GetAllDepartmentNames()
        {
            DataTable table = new DataTable();

            using (StreamReader r = new StreamReader(Path.Combine(Environment.CurrentDirectory, @"MocDB", "department.json")))
            {
                string json = r.ReadToEnd();
                table = (DataTable)JsonConvert.DeserializeObject(json, (typeof(DataTable)));
            }

            return new JsonResult(table);
        }

        [NonAction]
        public IEnumerable<Employee> GetAllEmp()
        {
            List<Employee> departments = new List<Employee>();
            JSONReadWrite readWrite = new JSONReadWrite();
            departments = JsonConvert.DeserializeObject<List<Employee>>(readWrite.Read("Employee.json", "MocDB"));
            return departments;
        }
    }
}
