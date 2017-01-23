﻿using System.Web.Mvc;
using System.Web.Security;

using MVC_Sample.Models.ViewModels;

using Touryo.Infrastructure.Business.Presentation;
using Touryo.Infrastructure.Business.Util;
using Touryo.Infrastructure.Framework.Util;


namespace MVC_Sample.Controllers
{
    public class HomeController : MyBaseMVController
    {
        /// <summary>
        /// GET: Home
        /// </summary>
        /// <returns>ActionResult</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// GET: /Home/Login
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        public ActionResult Login()
        {
            // Session消去
            this.FxSessionAbandon();

            return this.View();
        }

        /// <summary>
        /// POST: /Home/Login
        /// </summary>
        /// <param name="model">LoginViewModel</param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (!string.IsNullOrEmpty(model.UserName))
            {
                // 認証か完了した場合、認証チケットを生成し、元のページにRedirectする。
                // 第２引数は、「クライアントがCookieを永続化（ファイルとして保存）するかどうか。」
                // を設定する引数であるが、セキュリティを考慮して、falseの設定を勧める。
                FormsAuthentication.RedirectFromLoginPage(model.UserName, false);

                // 認証情報を保存する。
                MyUserInfo ui = new MyUserInfo(model.UserName, Request.UserHostAddress);
                UserInfoHandle.SetUserInformation(ui);

                //基盤に任せるのでリダイレクトしない。
                //return this.Redirect(ReturnUrl);
                return new EmptyResult();
            }
            else
            {
                // ユーザー認証 失敗
                this.ModelState.AddModelError(string.Empty, "指定されたユーザー名またはパスワードが正しくありません。");

                // Session消去
                this.FxSessionAbandon();

                // ポストバック的な
                return this.View(model);
            }
        }

        /// <summary>
        /// Get: /Home/Logout
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return this.Redirect(Url.Action("Index", "Home"));
        }
    }
}