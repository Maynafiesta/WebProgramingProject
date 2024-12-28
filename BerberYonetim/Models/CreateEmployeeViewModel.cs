using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models;

public class CreateEmployeeViewModel
{
    [Required]
    public string Name { get; set; } // Çalışan adı

    public List<int> SelectedSkillIds { get; set; } = new List<int>(); // Seçilen yeteneklerin ID'leri

    public List<Skill> AvailableSkills { get; set; } = new List<Skill>(); // Mevcut yetenekler
}