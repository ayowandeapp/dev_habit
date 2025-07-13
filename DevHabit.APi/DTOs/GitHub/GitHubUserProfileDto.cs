using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevHabit.APi.DTOs.GitHub
{
    public sealed class GitHubUserProfileDto
    {
    public string Login { get; set; }              
    public long Id { get; set; }                    
    public string AvatarUrl { get; set; }           
    public string Url { get; set; }                 
    public string Name { get; set; }                
    public string Email { get; set; }               
    public string Location { get; set; }           
    public string Bio { get; set; }                 
    public int PublicRepos { get; set; }           
    public int Followers { get; set; }              
    public int Following { get; set; }              
    public DateTime CreatedAt { get; set; }        
    public DateTime UpdatedAt { get; set; } 
        
    }
}