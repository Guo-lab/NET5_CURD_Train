# NET5.Core Building Process

> [Asp.Net Core](https://blog.csdn.net/stone0823/article/details/118796624?spm=1001.2101.3001.6650.3&utm_medium=distribute.pc_relevant.none-task-blog-2%7Edefault%7EESLANDING%7Edefault-3-118796624-blog-118890054.pc_relevant_landingrelevant&depth_1-utm_source=distribute.pc_relevant.none-task-blog-2%7Edefault%7EESLANDING%7Edefault-3-118796624-blog-118890054.pc_relevant_landingrelevant&utm_relevant_index=4)

1. Install SQLServer, Visual Studio, (SVN) and SSMS
2. SVN Init, 运行项目的签出以及BaseLibs的签出, 对于基本库和运行项目软连接的建立, 建立分支可在待fork目录右键fork到新分支（文件夹建立软连接的时候，目标文件夹不可存在，但是其父目录需完整）
3. ESC5.Web\NHibernate.config中配置了数据库连接串, 因此SSMS可以进行配置
4. SSMS配置: (1) 先建立SQL Server 用户(PUBLIC + SYSADMIN)，通过服务器SQL Server验证 (2) 后进行数据库还原，bak备份导入
5. 数据库字符串设置

   ```
   <property name="connection.connection_string">Data Source=.;Database=Train_Gsq;User ID=admin;Password=123456</property>
   <property name="default_schema">Train_Gsq.dbo</property>
   ```

   [DEFAULT_SCHEMA](https://learn.microsoft.com/en-us/sql/t-sql/statements/alter-user-transact-sql?view=sql-server-ver16)
6. 利用IIS Express运行sln工程

(cshtml 中的 Input 需要在 VM 中到单独定义)

<br> <br><br>

# 为清单增加数据库 field 增加查询条件

1. 增添 CURD 功能，涉及到（以增添筛选功能为例）Controller, Edit.ctrl.js 与 Edit.cshtml 在 MVC 的框架下调整
2. 增加数据库的表的 field， 创建数据库，创建表在SSMS 或 SQL Server 中操作即可

   [Add Columns to a Table (Database Engine) - SQL Server | Microsoft Learn](https://learn.microsoft.com/en-us/sql/relational-databases/tables/add-columns-to-a-table-database-engine?view=sql-server-ver16)

   ```sql
   SELECT * FROM dbo.TR_User

   ALTER TABLE dbo.TR_User
   ADD Mood VARCHAR(5) NULL;
   ```

   [SQL Server数据类型 - SQL Server教程 (yiibai.com)](https://www.yiibai.com/sqlserver/sqlserver_datatypes.html)[Create tables (Database Engine) - SQL Server | Microsoft Learn](https://learn.microsoft.com/en-us/sql/relational-databases/tables/create-tables-database-engine?view=sql-server-ver16)

   ```sql
   CREATE TABLE Train_Gsq.dbo.TR_Customer(
       Id           INT            NOT NULL     PRIMARY KEY,
   	Email        VARCHAR (50)   NOT NULL,
   	Name_        VARCHAR (10)   NOT NULL,
   	Gender       TINYINT        NOT NULL,
   	RegisterDate SMALLDATETIME      NULL,
   	Spending     DECIMAL(10, 3) NOT NULL,
   	Vip          TINYINT        NOT NULL,
   	Active       BIT            NOT NULL,
   );


   INSERT INTO Train_Gsq.dbo.TR_Customer VALUES('88', '123@qq.com', 'Gsq', '0', '', '100.000', '3', '1');

   UPDATE Train_Gsq.dbo.TR_Customer SET RegisterDate = NULL WHERE Name_ = 'Gsq';

   SELECT * FROM Train_Gsq.dbo.TR_Customer;
   ```

   建表之后，处理 CURD 功能
3. [第 3 部分，将视图添加到 ASP.NET Core MVC 应用 | Microsoft Learn](https://learn.microsoft.com/zh-cn/aspnet/core/tutorials/first-mvc-app/adding-view?view=aspnetcore-3.1&tabs=visual-studio)

> 规范编程 CoC?  配置 => 规范
> 三个类库 -> MVC

依靠规范，在创建文件的时候新建普通文件即可，通过规范“编译链接”，会在 Web（cshtml，js）bin 集中 ddl

<br>

- Bugs: "sqlserver: 不能将值 NULL 插入列 'id'" + "阻止保存要求或者重新创建表"
  Solution:

  - 数据库表邮右键设计，修改标识规范
  - [不允许保存更改的错误消息 - SQL Server | Microsoft Learn](https://learn.microsoft.com/zh-cn/troubleshoot/sql/ssms/error-when-you-save-table)
- Bugs: Error: kendo grid创建失败，请检查属性/参数设置是否正确、列设置的列数与tablehead中的列数是否一致 + Cannot read properties of undefined (reading 'replace')
  Solution: 命名需一致 （+ 有参数 undefined）

  ``````html
  <table kendo-grid="c.grdUser" k-data-source="DataSource" options="GridOptions" pb-kendo-grid="c.vm" 
  pb-kendo-grid-cols="['Email',
      'Name_,T,70px|{style: \'text-align: center;background-color:lightgreen\'}',
      'Gender,,80px', 'RegisterDate', 'Spending', 'Vip', 'Active', ',OT', ]" 
  class="myTable">
  <!-- 'Vip, | Display: \'Customer_RankEnum\'',-->
  <!-- Or 'Vip, | Dict: \'Customer_RankEnum\'',-->
  </table>
      ...
  <script Id="Name_-template" type="text/x-kendo-template">
      <a class="a-link" @Url.State("forward:Edit(Id:#=Id#)")>#=Name_#</a>
  </script>

  <script Id="Operation-template" type="text/x-kendo-template">
      <kendo-button type="button" icon="'k-icon k-i-delete'" ng-click="c.Delete_click(#=Id#,'#=Name_#')">
          <span translate="Delete"></span>
      </kendo-button>
  </script>
  ``````

  对于 kendo 命名 DisplayExtension.cs
  对于 Enum  命名 ListBuilder.cs
  对于标识栏 命名 zh-cn, App_Dict + Scripts/lang/dict.js
  改了一通，不知道怎么回事就过了...

> Debug: [Debug.WriteLine 方法 (System.Diagnostics) | Microsoft Learn](https://learn.microsoft.com/zh-cn/dotnet/api/system.diagnostics.debug.writeline?view=net-8.0)

<br>
<br>
<br>

# 新任务：加外键√，增添搜索条件√，

# 多行编辑，DTO查询，增添提示确认方式

1. 加外键 [SQL FOREIGN KEY 约束 | 菜鸟教程 (runoob.com)](https://www.runoob.com/sql/sql-foreignkey.html)
2. 在外键的情况下，进行联动，通过 DORef 的 list 和 map 可以在第二个表中选取第一个表中的对象，BD需要提前获取RefList，否则下拉的时候无法找到 List

   ```cs
   public ActionResult Edit(int Id) {
       var m = new CustomerEditVM()
       {
           UserList = UserBD.GetRefList()
       };
       m.Input = CustomerBD.GetDto<CustomerEditVM.EditInput>(Id);
       return ForView(m);
   }
   ```
3.
4. 确认方式 AjaxSubmit内置确认

   ```js
       c.AjaxSubmit('c.frmEdit', null, { confirm: '' }, function (r) {
           if (r.IsNoop) {
               pb.AjaxNavBack();
           }
       });
   ```
   确认方式 pbui.Confirm

   ```js
       c.Delete_click = function (id, name) {
           pbui.Confirm('ConfirmDelete', name).then(confirmed => {
               if (confirmed) {
                   c.AjaxSubmit('c.frmSearch', 'Id=' + id, {url: 'Delete'}, function (r) {
                       c.grdCustomer.Bind(r.data);
                   });
               }
           });
       };
   ```
