using FitnessHubMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace FitnessHubMVC.Controllers
{
    public class MemberController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            HttpClient client = new HttpClient();
            var response = client.GetAsync("https://localhost:7201/api/Member/GetMembers").Result;
            List<Member> members = JsonConvert.DeserializeObject<List<Member>>(response.Content.ReadAsStringAsync().Result);
            return View(members);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Member());
        }

        [HttpPost]
        public IActionResult Create(Member member)
        {
            HttpClient client = new HttpClient();
            StringContent content = new StringContent(JsonConvert.SerializeObject(member), System.Text.Encoding.UTF8, "application/json");
            var response = client.PostAsync("https://localhost:7201/api/Member/AddMembers", content).Result;
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            HttpClient client = new HttpClient();
            var response = client.GetAsync($"https://localhost:7201/api/Member/GetMembersById/{id}").Result;
            var members = JsonConvert.DeserializeObject<Member>(response.Content.ReadAsStringAsync().Result);
            return View(members);
        }

        [HttpPost]
        public IActionResult Edit(Member member)
        {
            HttpClient client = new HttpClient();
            StringContent content = new StringContent(JsonConvert.SerializeObject(member), System.Text.Encoding.UTF8, "application/json");
            var response = client.PutAsync($"https://localhost:7201/api/Member/UpdateMembers/{member.MemberId}", content).Result;
            TempData["SuccessMessage"] = "Kayıt başarıyla güncellendi!";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            HttpClient client = new HttpClient();
            var response = client.GetAsync($"https://localhost:7201/api/Member/GetMembersById/{id}").Result;
            var members = JsonConvert.DeserializeObject<Member>(response.Content.ReadAsStringAsync().Result);
            return View(members);
        }
        [HttpPost]
        public IActionResult Delete(Member member)
        {
            HttpClient client = new HttpClient();
            var response = client.DeleteAsync($"https://localhost:7201/api/Member/DeleteMembers/{member.MemberId}").Result;
            TempData["SuccessMessage"] = "Kayıt başarıyla silindi!";
            return RedirectToAction("Index");
        }

    }
}
