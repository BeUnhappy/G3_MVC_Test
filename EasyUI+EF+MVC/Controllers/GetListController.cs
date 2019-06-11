using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasyUI_EF_MVC.Models;

namespace EasyUI_EF_MVC.Controllers
{
    public class GetListController : Controller
    {
        // GET: GetList
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetList(int page, int rows, string name, int? deptId)
        //可能为空值用?
        {
            using (G3TextEntities db = new G3TextEntities())
            {
                //Request.QueryString
                //Request.Form
                //ASP.NET取值方式

                //查询员工表
                var lst = db.T_Emp.ToList()
                    .Select(c => new
                    {
                        //返回匿名对象
                        emp_id = c.EmpId,
                        emp_name = c.EmpName,
                        emp_sex = c.EmpSex,
                        emp_age = c.EmpAge,
                        emp_birthday = c.EmpBirthday.ToString("yyyy-MM-dd"),
                        dept_name = c.T_Dept.DeptName,
                        dept_id = c.DeptId,
                    }).ToList();
                //判断搜索框字符串不为空----查询功能
                if (!string.IsNullOrEmpty(name))
                {
                    lst = lst.Where(c => c.emp_name.Contains(name)).ToList();
                }
                //判断下拉列表不为空，是否有值----查询功能
                if (deptId.HasValue)
                {
                    lst = lst.Where(c => c.dept_id == deptId).ToList();
                };
                //创建匿名对象
                var obj = new
                {
                    //EF分页
                    tatal = lst.Count(),
                    rows = lst.Skip((page - 1) * rows).Take(rows).ToList()
                };
                //return Json(lst, JsonRequestBehavior.AllowGet);
                //采用分页
                return Json(obj, JsonRequestBehavior.AllowGet);
            }
        }
        //查询部门
        public JsonResult GetDept()
        {
            using (G3TextEntities db = new G3TextEntities())
            {
                var lst = db.T_Dept.Select(d => new
                {
                    dept_id = d.DeptId,
                    dept_name = d.DeptName
                }).ToList();
                return Json(lst, JsonRequestBehavior.AllowGet);
            }
        }
        //新增功能
        [HttpPost]
        public JsonResult CreateEmp(T_Emp emp)
        {
            using (G3TextEntities db = new G3TextEntities())
            {
                db.T_Emp.Add(emp);
                int rs = db.SaveChanges();
                if (rs > 0)
                {
                    var obj = new { msg = "新增成功..." };
                    return Json(obj);
                }
                else
                {
                    var obj = new { msg = "新增失败..." };
                    return Json(obj);
                }
            }
        }
        //修改功能
        public JsonResult GetEmp(int id)
        {
            using (G3TextEntities db = new G3TextEntities())
            {
                var obj = db.T_Emp.Where(c => c.EmpId == id).ToList()
                    .Select(c => new
                    {
                        emp_id = c.EmpId,
                        emp_name = c.EmpName,
                        emp_sex = c.EmpSex,
                        emp_age = c.EmpAge,
                        emp_birthday = c.EmpBirthday.ToString("yyyy-MM-dd"),
                        dept_id = c.DeptId,
                    }).FirstOrDefault();
                return Json(obj, JsonRequestBehavior.AllowGet);
            }
        }
        //删除功能
        public JsonResult DeleteEmp(List<int> IdList)
        {
            using (G3TextEntities db = new G3TextEntities())
            {
                //包含集合
                //先查询
                var lst = db.T_Emp.Where(c => IdList.Contains(c.EmpId)).ToList();
                foreach (var item in lst)
                {
                    //移除
                    db.T_Emp.Remove(item);
                }
                int rs = db.SaveChanges();
                if (rs > 0)
                {
                    var obj = new { status = "success", msg = "删除成功..." };
                    return Json(obj, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var obj = new { status = "error", msg = "删除失败..." };
                    return Json(obj, JsonRequestBehavior.AllowGet);
                }
            }
        }
    }
}