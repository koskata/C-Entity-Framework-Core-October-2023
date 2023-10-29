using System.Text;

using Microsoft.EntityFrameworkCore;

using SoftUni.Data;
using SoftUni.Models;

namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            SoftUniContext softUniContext = new SoftUniContext();

            Console.WriteLine(RemoveTown(softUniContext));
        }

        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            var employees = context.Employees
                            .OrderBy(e => e.EmployeeId)
                            .Select(e => new
                            {
                                e.FirstName,
                                e.LastName,
                                e.MiddleName,
                                e.JobTitle,
                                e.Salary
                            });

            string result = string.Join(Environment.NewLine, employees.Select(e => $"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.Salary:f2}"));

            return result;
        }


        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            var employees = context.Employees
                            .Select(e => new
                            {
                                e.FirstName,
                                e.Salary
                            })
                            .Where(e => e.Salary > 50000)
                            .OrderBy(e => e.FirstName);

            string result = string.Join(Environment.NewLine, employees.Select(e => $"{e.FirstName} - {e.Salary:f2}"));

            return result;
        }

        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            var employees = context.Employees
                            .Select(e => new
                            {
                                e.FirstName,
                                e.LastName,
                                e.Department.Name,
                                e.Salary
                            }).Where(e => e.Name == "Research and Development")
                            .OrderBy(e => e.Salary).ThenByDescending(e => e.FirstName);

            string result = string.Join(Environment.NewLine, employees.Select(e => $"{e.FirstName} {e.LastName} from {e.Name} - ${e.Salary:f2}"));

            return result;
        }

        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            Address address = new Address
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            Employee emplNakov = context.Employees.Where(e => e.LastName == "Nakov").First();

            emplNakov.Address = address;

            context.SaveChanges();

            var employees = context.Employees
                            .OrderByDescending(e => e.AddressId)
                            .Select(e => new
                            {
                                e.Address.AddressText
                            }).Take(10);

            string result = string.Join(Environment.NewLine, employees.Select(e => $"{e.AddressText}"));

            return result;
        }

        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            var employees = context.Employees
                .Select(e => new
                {
                    FullName = e.FirstName + " " + e.LastName,
                    ManagerFullName = e.Manager.FirstName + " " + e.Manager.LastName,
                    Projects = e.EmployeesProjects.Where(e => e.Project.StartDate.Year >= 2001 && e.Project.StartDate.Year <= 2003)
                    .Select(ep => new
                    {
                        ProjectName = ep.Project.Name,
                        StartDate = ep.Project.StartDate.ToString("M/d/yyyy h:mm:ss tt"),
                        EndDate = ep.Project.EndDate != null
                        ? ep.Project.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt")
                        : "not finished"
                    })
                }).Take(10).ToList();

            StringBuilder sb = new StringBuilder();
            foreach (var empl in employees)
            {
                sb.AppendLine($"{empl.FullName} - Manager: {empl.ManagerFullName}");
                if (empl.Projects is not null)
                {
                    foreach (var project in empl.Projects)
                    {
                        sb.AppendLine($"--{project.ProjectName} - {project.StartDate} - {project.EndDate}");
                    }
                }
            }

            string result = string.Join(Environment.NewLine, sb.ToString().TrimEnd());

            return result;
        }


        public static string GetAddressesByTown(SoftUniContext context)
        {
            var addresses = context.Addresses
                            .Select(a => new
                            {
                                a.AddressText,
                                a.Town.Name,
                                a.Employees.Count
                            }).OrderByDescending(a => a.Count).ThenBy(a => a.Name).ThenBy(a => a.AddressText).Take(10);

            string result = string.Join(Environment.NewLine, addresses.Select(a => $"{a.AddressText}, {a.Name} - {a.Count} employees"));

            return result;
        }

        public static string GetEmployee147(SoftUniContext context)
        {
            var employee147 = context.Employees.Select(e => new
            {
                e.EmployeeId,
                e.FirstName,
                e.LastName,
                e.JobTitle,
                Projects = e.EmployeesProjects
                    .Select(ep => new { ProjectName = ep.Project.Name })
                    .OrderBy(p => p.ProjectName)
                    .ToArray()
            })
            .FirstOrDefault(e => e.EmployeeId == 147);


            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"{employee147.FirstName + " " + employee147.LastName} - {employee147.JobTitle}");
            foreach (var project in employee147.Projects)
            {
                sb.AppendLine(project.ProjectName);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            var departments = context.Departments
                                .Select(d => new
                                {
                                    d.Name,
                                    FullName = d.Manager.FirstName + " " + d.Manager.LastName,
                                    d.Employees.Count,
                                    Employees = d.Employees
                                        .Select(e => new
                                        {
                                            EmplFirstName = e.FirstName,
                                            EmplLastName = e.LastName,
                                            EmplJobTitle = e.JobTitle
                                        })
                                        .OrderBy(e => e.EmplFirstName)
                                        .ThenBy(e => e.EmplLastName).ToList()
                                }).Where(d => d.Count > 5)
                                .OrderBy(d => d.Count).ThenBy(d => d.Name).ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var dep in departments)
            {
                sb.AppendLine($"{dep.Name} - {dep.FullName}");
                foreach (var empl in dep.Employees)
                {
                    sb.AppendLine($"{empl.EmplFirstName + " " + empl.EmplLastName} - {empl.EmplJobTitle}");
                }
            }



            //string result = string.Join(Environment.NewLine, departments.Select(d => $"{d.Name} - {d.FullName}"));

            return sb.ToString().TrimEnd();
        }

        public static string GetLatestProjects(SoftUniContext context)
        {
            var projects = context.Projects
                            .Select(p => new
                            {
                                p.Name,
                                p.Description,
                                p.StartDate
                            }).OrderByDescending(p => p.StartDate).Take(10).ToList();

            projects = projects.OrderBy(p => p.Name).ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var p in projects)
            {
                sb.AppendLine(p.Name);
                sb.AppendLine(p.Description);
                sb.AppendLine(p.StartDate.ToString("M/d/yyyy h:mm:ss tt"));
            }

            return sb.ToString().TrimEnd();
        }

        public static string IncreaseSalaries(SoftUniContext context)
        {
            var employees = context.Employees
                            .Select(e => new
                            {
                                e.FirstName,
                                e.LastName,
                                Salary = e.Salary * 1.12m,
                                e.Department.Name
                            }).Where(e => e.Name == "Engineering" || e.Name == "Tool Design" || e.Name == "Marketing" || e.Name == "Information Services")
                            .OrderBy(e => e.FirstName).ThenBy(e => e.LastName).ToList();

            string result = string.Join(Environment.NewLine, employees.Select(e => $"{e.FirstName + " " + e.LastName} (${e.Salary:f2})"));

            return result;
        }

        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            var employees = context.Employees
                            .Select(e => new
                            {
                                e.FirstName,
                                e.LastName,
                                e.JobTitle,
                                e.Salary
                            }).Where(e => e.FirstName.StartsWith("Sa"))
                            .OrderBy(e => e.FirstName).ThenBy(e => e.LastName);

            string result = string.Join(Environment.NewLine, employees.Select(e => $"{e.FirstName + " " + e.LastName} - {e.JobTitle} - (${e.Salary:f2})"));

            return result;
        }

        public static string DeleteProjectById(SoftUniContext context)
        {
            var project = context.Projects.Find(2);

            context.Projects.Remove(project);

            var emplProjectsToRemove = context.EmployeesProjects.Where(e => e.ProjectId == 2).ToArray();

            

            context.SaveChanges();

            var projects = context.Projects.Select(e => e.Name).Take(10);

            return string.Join(Environment.NewLine, projects);
        }

        public static string RemoveTown(SoftUniContext context)
        {
            var townToDelete = context.Towns.FirstOrDefault(e => e.Name == "Seattle");
            var addressesToDelete = context.Addresses.Where(a => a.Town.Name == "Seattle");

            int count = addressesToDelete.Count();

            var employees = context.Employees.Where(e => addressesToDelete.Any(a => a.AddressId == e.AddressId));

            foreach (var employee in employees)
            {
                employee.AddressId = null;
            }

            context.RemoveRange(addressesToDelete);
            context.Towns.Remove(townToDelete);

            context.SaveChanges();

            var sb = new StringBuilder();

            sb.AppendLine($"{count} addresses in {townToDelete.Name} were deleted");

            return sb.ToString().TrimEnd();
        }
    }
}