using EmployeePerformance.Models;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class LeaveRequest

{
	[Key]
	public int LeaveId { get; set; }

    [Required]
    [ForeignKey("Employee")]
    public int EmployeeId { get; set; }
	public Employee Employee { get; set; }
	[Required]
	[MaxLength(50)]
	public string LeaveType { get; set; }
	[Required]
	public DateTime StartgDate { get; set; }
	[Required]
	public DateTime EndDate { get; set; }
	public string? Reason { get; set; }
	[Required]
	[MaxLength(20)]
	public string Status { get; set; } = "Pending";
	[Required]
	public DateTime RequestDone { get; set; } = DateTime.UtcNow;



	
}
