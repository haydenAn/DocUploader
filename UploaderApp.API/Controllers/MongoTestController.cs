using UploaderApp.API.Models;
using UploaderApp.API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using UploaderApp.API.Data;
using UploaderApp.API.Helpers;
using System.Threading.Tasks;
using System;
using System.Net;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;


namespace UploaderApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocInfoController : ControllerBase
    {
        private readonly DocInfoService _docInfoService;

        public DocInfoController(DocInfoService docInfoService)
        {
            _docInfoService = docInfoService;
        }

        //api/docinfo
        [HttpGet]
        [HttpGet("{filter}")]
        public async Task<IActionResult> GetDocInfo([FromQuery] ReportParams rptParams) {
            
           PagedList<DocInfo> docs;
           if(rptParams.Keys == null){
              docs = await _docInfoService.GetDocInfo(rptParams);
           }
           
           docs = await _docInfoService.GetDocsFilteredResult(rptParams);

           if(docs!= null){
              Response.AddPagination(docs.CurrentPage, docs.PageSize, docs.TotalCount, docs.TotalPages);
           }
           return Ok(docs);
        }
        ////for keyword searching only as
        
        [HttpPost]
        public IActionResult CreatDocInfo(DocInfo doc)
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

            var result = _docInfoService.CreatDocInfo(doc);

            if ( result!= null )
            {
                string from = "johnmik35@hotmail.com";
                string to = doc.EmailAddress;
                string subject = "Master Sales Agreement for IndxLogic";

                MailMessage msg = CreateMsg(doc, from, to, subject);
                SendMsg(msg);
                // id = doc.Id.ToString() 
                 return Created(nameof(GetEmailLink), new {  emaillink = sLink , id = doc.Id.ToString() , document = doc});
            }

            return BadRequest("Error saving document/ email link info to database");
        }
        #region emailMsgHandler

        [ActionName(nameof(GetEmailLink))]
        [HttpGet("/emaillink", Name="GetEmailLink")]
        public IActionResult GetEmailLink(string emaillink)
        {
            return Ok(emaillink);
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

        private MailMessage CreateMsg(DocInfo doc, string from, string to, string subject)
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
        #endregion
        // [HttpPut("{id}")]
        // public IActionResult Update(string id, DocInfo docInfoIn)
        // {
        //     var docInfo = _docInfoService.GetAllDocs(id);

        //     if (docInfo == null)
        //     {
        //         return NotFound();
        //     }

        //     _docInfoService.Update(id, docInfoIn);

        //     return NoContent();
        // }

        // [HttpDelete("{id)}")]
        // public IActionResult Delete(string id)
        // {
        //     var docInfo = _docInfoService.GetAllDocs(id);

        //     if (docInfo == null)
        //     {
        //         return NotFound();
        //     }

        //     _docInfoService.Remove(docInfo.Id);

        //     return NoContent();
        // }
    }
}