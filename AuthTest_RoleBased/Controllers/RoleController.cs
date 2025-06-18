using AuthTest_RoleBased.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthTest_RoleBased.Controllers
{
    //[Authorize(Roles = "ADMIN")]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public RoleController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        // =======================
        // Role Management Section
        // =======================
        public IActionResult Index()
        {
            ViewBag.roleList = _roleManager.Roles.ToList();
            ViewBag.msg = TempData["msg"];
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(string userRole)
        {
            string msg = "";

            if (!string.IsNullOrWhiteSpace(userRole))
            {
                if (await _roleManager.RoleExistsAsync(userRole))
                {
                    msg = $"Role [{userRole}] already exists!";
                }
                else
                {
                    IdentityRole role = new IdentityRole(userRole.Trim());
                    var result = await _roleManager.CreateAsync(role);
                    msg = result.Succeeded
                        ? $"Role [{userRole}] has been created successfully!"
                        : $"Failed to create role [{userRole}]";
                }
            }
            else
            {
                msg = "Please enter a valid role name!";
            }

            TempData["msg"] = msg;
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult EditRole(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                TempData["msg"] = "Invalid role selected.";
                return RedirectToAction(nameof(Index));
            }

            return View("EditRole", roleName);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(string oldRoleName, string newRoleName)
        {
            if (string.IsNullOrWhiteSpace(oldRoleName) || string.IsNullOrWhiteSpace(newRoleName))
            {
                TempData["msg"] = "Invalid role names.";
                return RedirectToAction(nameof(Index));
            }

            var role = await _roleManager.FindByNameAsync(oldRoleName);
            if (role == null)
            {
                TempData["msg"] = $"Role [{oldRoleName}] not found!";
                return RedirectToAction(nameof(Index));
            }

            if (await _roleManager.RoleExistsAsync(newRoleName))
            {
                TempData["msg"] = $"Role [{newRoleName}] already exists!";
                return RedirectToAction(nameof(Index));
            }

            role.Name = newRoleName;
            role.NormalizedName = newRoleName.ToUpper();

            var result = await _roleManager.UpdateAsync(role);

            TempData["msg"] = result.Succeeded
                ? $"Role updated from [{oldRoleName}] to [{newRoleName}]"
                : "Failed to update role.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string roleName)
        {
            string msg = "";

            if (!string.IsNullOrWhiteSpace(roleName))
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    var result = await _roleManager.DeleteAsync(role);
                    msg = result.Succeeded
                        ? $"Role [{roleName}] deleted successfully!"
                        : $"Failed to delete role [{roleName}]";
                }
                else
                {
                    msg = $"Role [{roleName}] not found!";
                }
            }
            else
            {
                msg = "Invalid role name!";
            }

            TempData["msg"] = msg;
            return RedirectToAction(nameof(Index));
        }

        // =======================
        // Role Assignment Section
        // =======================
        public IActionResult AssignRole()
        {
            ViewBag.users = _userManager.Users.ToList();
            ViewBag.roles = _roleManager.Roles.ToList();
            ViewBag.msg = TempData["msg"];

            var userRolesList = _userManager.Users
                .Select(user => new
                {
                    user.Email,
                    Roles = _userManager.GetRolesAsync(user).Result.ToList()
                })
                .ToList();

            ViewBag.userRoles = userRolesList;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(string userData, string roleData)
        {
            string msg = "";

            if (!string.IsNullOrEmpty(userData) && !string.IsNullOrEmpty(roleData))
            {
                ApplicationUser user = await _userManager.FindByEmailAsync(userData);
                if (user != null)
                {
                    if (await _roleManager.RoleExistsAsync(roleData))
                    {
                        if (!await _userManager.IsInRoleAsync(user, roleData))
                        {
                            var result = await _userManager.AddToRoleAsync(user, roleData);
                            msg = result.Succeeded
                                ? "Role has been assigned to the user!"
                                : "Failed to assign role!";
                        }
                        else
                        {
                            msg = $"User already has the role [{roleData}]!";
                        }
                    }
                    else
                    {
                        msg = "Selected role does not exist!";
                    }
                }
                else
                {
                    msg = "Selected user not found!";
                }
            }
            else
            {
                msg = "Please select both a user and a role.";
            }

            TempData["msg"] = msg;
            return RedirectToAction(nameof(AssignRole));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveUserRole(string userEmail, string roleName)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null || !await _userManager.IsInRoleAsync(user, roleName))
            {
                TempData["msg"] = "Invalid user or role.";
                return RedirectToAction(nameof(AssignRole));
            }

            var result = await _userManager.RemoveFromRoleAsync(user, roleName);
            TempData["msg"] = result.Succeeded
                ? $"Removed role [{roleName}] from user [{userEmail}]"
                : "Failed to remove role.";

            return RedirectToAction(nameof(AssignRole));
        }

        [HttpPost]
        public async Task<IActionResult> EditUserRole(string userEmail, string oldRole, string newRole)
        {
            if (string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(oldRole) || string.IsNullOrEmpty(newRole))
            {
                TempData["msg"] = "Invalid role change request.";
                return RedirectToAction(nameof(AssignRole));
            }

            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null || !await _userManager.IsInRoleAsync(user, oldRole))
            {
                TempData["msg"] = "User/Old role mismatch.";
                return RedirectToAction(nameof(AssignRole));
            }

            if (!await _roleManager.RoleExistsAsync(newRole))
            {
                TempData["msg"] = $"New role [{newRole}] does not exist.";
                return RedirectToAction(nameof(AssignRole));
            }

            var removeResult = await _userManager.RemoveFromRoleAsync(user, oldRole);
            var addResult = await _userManager.AddToRoleAsync(user, newRole);

            if (removeResult.Succeeded && addResult.Succeeded)
            {
                TempData["msg"] = $"User [{userEmail}] role changed from [{oldRole}] to [{newRole}].";
            }
            else
            {
                TempData["msg"] = "Failed to change user role.";
            }

            return RedirectToAction(nameof(AssignRole));
        }
    }
}
