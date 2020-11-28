using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UploaderApp.API.Data;
using UploaderApp.API.Dtos;
using UploaderApp.API.Helpers;
using UploaderApp.API.Models;

namespace UploaderApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LicensingController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly ISendLinkRepository _repo;
        public LicensingController(DataContext context, ISendLinkRepository repo)
        {
            _repo = repo;
            _context = context;
        }

        /// <summary>
        /// Retrieves the specified Document using the unique emaillinkid
        /// </summary>
        /// <param name="emaillink"></param> 
        // Get api/licensing/1010101
        [HttpGet("{emaillink}")]
        public IActionResult GetDocInfo(string emaillink)
        {
            string s1 = "not found";
            var doc = _repo.GetEmailLink(new Guid(), emaillink).Result;
            if (doc!=null)
            {
                s1 = $"Sent {doc.DocumentFullName} to {doc.FirstName} {doc.LastName} at {doc.EmailAddress} at company {doc.Company}";
                return Ok(doc);
            }
            return NoContent();
        }

        /// <summary>
        /// view-doc/{id} updates the date viewed field
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Ok</returns>
        [HttpPut("view-doc/{id}")]
        public async Task<IActionResult> UpdateDateViewed(int id)
        {
            var doc = _repo.GetDocumentInfoById(id).Result;            
            if (doc != null)
            {
                doc.dateViewed = DateTime.Now;
                doc.Status = Status.Viewed.ToString();
                if (await _repo.SaveAll())
                {
                    return Ok();
                }
            }
            return BadRequest($"Error updating dateViewed");
        }

        /// <summary>
        /// confirm-doc/{id} updates the date confirmed field, and sends a confirmation email to the end user
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Ok</returns>
        [HttpPut("confirm-doc/{id}")]
        public async Task<IActionResult> ConfirmReceipt(int id)
        {
            var doc = _repo.GetDocumentInfoById(id).Result;            
            if (doc != null)
            {
                doc.dateAgreed = DateTime.Now;
                doc.Status = Status.Agreed.ToString();
                if (await _repo.SaveAll())
                {
                    string s1 = $"Confirming #{doc.Id} {doc.LastName} at {doc.EmailAddress} at company {doc.Company}";
                    string from = "johnmik35@hotmail.com";
                    string to = doc.EmailAddress;
                    string subject = "Confirmation email for " + "Master Sales Agreement for IndxLogic";

                    MailMessage msg = CreateConfirmationMsg(doc, from, to, subject);
                    SendMsg(msg);

                    return Ok();
                }
            }
            return BadRequest($"Cannot find id # {id}. Error confirming receipt");
        }

        /// <summary>
        /// Get report based on ReportParams and filter/ search 
        /// </summary>
        /// <param name="rptParams"></param>
        /// <param name="filter"></param>
        /// <returns>PagedList of DocumentInfo</returns>
        [HttpGet("report")]
        [HttpGet("report/{filter}")]
        public async Task<IActionResult> GetReport([FromQuery] ReportParams rptParams, string filter)
        {
            string search = "";
            string filter1 = HttpContext.Request.Query["filter"].ToString();
            string search1 = HttpContext.Request.Query["search"].ToString();
            Console.WriteLine("New get report call w/ filter=" + filter);
            // If there is some sort of filter or search
            if (!string.IsNullOrEmpty(filter)) { 
                // If there is a combo of filter+ search
                if (filter.Contains('+')) { 
                    string[] parts = filter.Split('+');
                    if (parts[0] == "Agreed" || parts[0]=="Viewed" || parts[0] == "Sent" || parts[0] == "Resent") {
                        filter = parts[0];
                        search = parts[1];
                    }
                    else { 
                        filter = parts[1];
                        search = parts[0];
                    }
                } // else if there is only a filter
                else if (filter == "Agreed" || filter=="Viewed" || filter == "Sent" || filter == "Resent") { 
                    search = ""; 
                }
                else { 
                    search = filter; 
                    filter = "";
                }
            }

            Console.WriteLine($"New GetReport call w/ filter={filter} and Search={search}");
            var docs = _repo.GetReport(rptParams, filter, search).Result;

            Response.AddPagination(docs.CurrentPage, docs.PageSize, docs.TotalCount, docs.TotalPages);

            return Ok(docs);
        }

        [HttpPost]
        public async Task<IActionResult> Post(DocumentInfo doc) //[FromBody] string value)
        {
            string s1 = $"Sent {doc.DocumentFullName} to {doc.FirstName} {doc.LastName} at {doc.EmailAddress} at company {doc.Company}";
            Debug.WriteLine(s1);
            // Add in useful values
            string sLink = GetNewEmailLinkId();
            doc.Description = sLink;
            doc.UniqueLinkId = sLink;
            doc.dateSent = DateTime.Now;
            doc.Status = Status.Sent.ToString();
            string[] files = doc.DocumentFullName.Split(';');
            // TESTING
            if (string.IsNullOrEmpty(doc.DocumentFullName))
                doc.DocumentFullName = "donttread.jfif";

            _repo.Add<DocumentInfo>(doc);   

            if (await _repo.SaveAll())
            {
                string from = "johnmik35@hotmail.com";
                string to = doc.EmailAddress;
                string subject = "Master Sales Agreement for IndxLogic";

                MailMessage msg = CreateMsg(doc, from, to, subject);
                SendMsg(msg);
                
                // CreatedAtRouteResult carr = CreatedAtRoute("GetEmailLink", new { emaillink = sLink });
                return Created("GetEmailLink", new { emaillink = sLink, id = doc.Id }); // carr;
            }

            return BadRequest("Error saving document/ email link info to database");
        }
        [HttpPost("test")]
                public async Task<IActionResult> Test(DocumentInfo doc)
{
    await  _repo.SaveAll();
    return Ok();
}

        /// <summary>
        /// Resend link to end user updates dateResent and (re)sends an email to end user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("resendlink/{id}")]
        public async Task<IActionResult> Post(int id)
        {
            var doc = _repo.GetDocumentInfoById(id).Result;
            if (doc!=null)
            {
                string s1 = $"Resending  {doc.DocumentFullName} to {doc.FirstName} {doc.LastName} at {doc.EmailAddress} at company {doc.Company}";
                string from = "johnmik35@hotmail.com";
                string to = doc.EmailAddress;
                string subject = "Resent link to " + "Master Sales Agreement for IndxLogic";

                MailMessage msg = CreateMsg(doc, from, to, subject);
                SendMsg(msg);

                doc.Description = "Sent originally at " + doc.dateSent.ToString();
                doc.dateResent = DateTime.Now;
                doc.Status = Status.Resent.ToString();

                await _repo.SaveAll();
                
                return Ok();
            }
            return BadRequest($"Cannot find id # {id}. Error resending email");
        }

        /// <summary>
        /// Internal GET call to support POST return 
        /// </summary>
        /// <param name="emaillink"></param>
        /// <returns></returns>
        [HttpGet("/emaillink", Name="GetEmailLink")]
        public IActionResult GetEmailLink(string emaillink)
        {
            return Ok(emaillink);
        }

        private bool SendMsg(MailMessage msg)
        {
            SmtpClient sv = new SmtpClient("smtp.gmail.com", 587);
            sv.EnableSsl = true;
            sv.Credentials = new NetworkCredential("cxmikalauskas@gmail.com", "So1omonOrange");

            try
            {
                sv.Send(msg);
                Console.WriteLine("email Sent");
            }
            catch (Exception ex)
            {
                Console.WriteLine("email not sent");
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

        private string signingBase = "localhost:5000/api/licensing/";

        private MailMessage CreateMsg(DocumentInfo doc, string from, string to, string subject)
        {
            string uniqueId = doc.UniqueLinkId;
            MailMessage msg = null;
            string body = "Hi. Thanks for your business. <br/> Please click this link to sign the licensing agreement. <br/> Your id is #" + uniqueId ;
            body += "<br/> Your url is " + doc.DocumentFullName + "<br/>";
            body += @"<button type='button' style ='background-color:#1800ff'>
                    <a style='font-size:20px;font-family:Arial;color:#ffffff;text-align:center;text-decoration:none;display:block;
                    background-color:#1800ff;border:1px solid #1800ff;padding:12px 32px;border-radius:3px' href='http://localhost:4200/sign-doc/" + uniqueId + @"'>Click to view
                    </a></button>";

            msg = new MailMessage(from, to, subject, body); //test 
            msg.IsBodyHtml = true;
            return msg;
        }

        private MailMessage CreateConfirmationMsg(DocumentInfo doc, string from, string to, string subject)
        {
            string uniqueId = doc.UniqueLinkId;
            MailMessage msg = null;
            string body = "Thank you for confirming reciept of the documents.<br/>The document are always available";
            body += "<br/> Your unique link is " + doc.DocumentFullName + "<br/>";
            body += @"<button type='button' style ='background-color:#1800ff'>
                    <a style='font-size:20px;font-family:Arial;color:#ffffff;text-align:center;text-decoration:none;display:block;
                    background-color:#1800ff;border:1px solid #1800ff;padding:12px 32px;border-radius:3px' href='http://localhost:4200/view-doc/" + uniqueId + @"'>Click to view
                    </a></button>";
            msg = new MailMessage(from, to, subject, body); //test 
            msg.IsBodyHtml = true;
            return msg;
        }

        private string GetNewEmailLinkId()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[16];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }
            // var UniqueID = new String(stringChars);
            return new String(stringChars);
        }

        // POST api/licensing/docs
        // [HttpPost("/docs")]
        // public IActionResult PostDocs([FromForm] FilesForUpload doc) //[FromBody] string value)
        // {
        //     var file = doc.File;

        //     if (file.Length > 0)
        //     {
        //         using (var stream = file.OpenReadStream())
        //         {
        //             // var uploadParams = new ImageUploadParams() { 
        //             //     File = new FileDescription(file.Name, stream),
        //             //     Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
        //             // };
        //             // uploadResult = _cloudinary.Upload(uploadParams);
        //         }
        //     }
        //     return Ok(file.Length.ToString());
        // }       

        // PUT api/licensing/5
        // [HttpPut("{id}")]
        // public void Put(int id, [FromBody] string value)
        // {
        // }

        // // DELETE api/licensing/5
        // [HttpDelete("{id}")]
        // public void Delete(int id)
        // {
        // }
    }
}