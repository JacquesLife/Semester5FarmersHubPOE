/// <summary>
/// Just basic getters and setters for the user role model.
/// This class is used to represent a user and their role in the system.
/// It contains properties for the user's ID, email, and current role.
/// </summary>

using System;

namespace Programming3A.Models;

public class UserRoleModel
{
    public string UserId { get; set; }
    public string Email { get; set; }
    public string CurrentRole { get; set; }
}

//--------------------------------------------------End of File-----------------------------------------------------