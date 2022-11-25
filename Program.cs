using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Reflection;

const string SITE_URL = @"https://url";
const string LOGIN_ID = "ユーザー";
const string LOGIN_PASSWORD = "パスワード";

using var driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location));

try
{
    #region サイトにログインする
    driver.Navigate().GoToUrl(SITE_URL);

    // 認証用のIDとパスワードの入力待ち（ポップアップなので自動化できない）
    Console.ReadKey();

    // ログインする
    driver.FindElement(By.Name("loginId")).SendKeys(LOGIN_ID);
    Thread.Sleep(2000);
    driver.FindElement(By.Name("loginPassword")).SendKeys(LOGIN_PASSWORD);
    Thread.Sleep(2000);
    driver.FindElement(By.Name("login")).Submit();
    Thread.Sleep(5000);
    #endregion

    #region スキル登録する
    driver.ExecuteScript("menuClick('50EE90F9F6DA40B3AD9A331C580A84E5','9203D5D7D2E64450A0793B7E8F45BD0C')");
    Thread.Sleep(3000);

    // 「スキル登録用」を選択
    new SelectElement(driver.FindElement(By.Name("frameworkId"))).SelectByIndex(1);
    Thread.Sleep(3000);

    // 「フレームワーク選択」を押下
    driver.FindElement(By.Id("frameworkSelBtn")).Click();
    Thread.Sleep(3000);

    // 「開発（xxx）」を選択
    // seleniumでjquery dynatreeを操作する方法が不明だったため、
    // 絞り込みなしで（全スキルを）スキル登録する。 ※特に問題ないはず…
    Thread.Sleep(5000);    // この間に手動選択してもOK

    // 「絞り込み」を押下
    driver.FindElement(By.Id("refineBtn")).Click();
    Thread.Sleep(3000);

    // ページの右側に表示されている一覧(テーブル)の「中タスク」の各リンクを巡る
    var checkedLinks = new Dictionary<string, string>();
    bool allChecked = false;
    while (true)
    {
        var table = driver.FindElement(By.ClassName("defaultTable"));
        var tds = table.FindElements(By.TagName("td"));
        foreach (var td in tds)
        {
            var id = td.GetAttribute("id");
            if (!string.IsNullOrEmpty(id))
            {
                // チェック済みのリンクはスキップする
                if (!checkedLinks.ContainsKey(id))
                {
                    checkedLinks.Add(id, td.Text);

                    try
                    {
                        // リンクにジャンプする
                        //driver.FindElement(By.Id(id)).Click();　この方法だとリンク先にジャンプしないものもあるため、hrefのjavascriptを直接実行する
                        driver.ExecuteScript($"goSkillEdit('{id}')");
                        Thread.Sleep(3000);

                        // 「確定」を押下
                        driver.FindElement(By.Id("confirmBtn")).Click();
                        Thread.Sleep(3000);
                    }
                    catch
                    {
                        // 要素が見つからなくても次に進む
                    }

                    break;
                }
            }
            // 全てのリンクを巡ったら終了する
            if (tds.Last().Equals(td))
            {
                allChecked = true;
                break;
            }
        }
        if (allChecked)
        {
            break;
        }
    }
    #endregion

    // キー入力待ち
    Console.ReadKey();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
