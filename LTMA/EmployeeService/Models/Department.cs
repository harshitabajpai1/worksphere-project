namespace EmployeeService.Models
{
    // Represents a department in the organization.
    public class Department
    {
        // Primary key of the department.
        public int Id { get; set; }

        // Department name, for example IT or Finance.
        public string Name { get; set; } = string.Empty;

        // A department can have many employees.
        public ICollection<Employee> Employees { get; set; }
            = new List<Employee>();
    }
}