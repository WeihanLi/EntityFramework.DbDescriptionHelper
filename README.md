# EntityFramework.DbDescriptionHelper

### Build Status
[![Build status](https://ci.appveyor.com/api/projects/status/yfd6oab5u5rw49ty/branch/master?svg=true)](https://ci.appveyor.com/project/WeihanLi/entityframework-dbdescriptionhelper/branch/master)

[![Build Status](https://travis-ci.org/WeihanLi/EntityFramework.DbDescriptionHelper.svg?branch=master)](https://travis-ci.org/WeihanLi/EntityFramework.DbDescriptionHelper)

### Intro
EntityFramework.DbDescriptionHelper,ef tool for generating database tables and columns description
(SqlServer only for now)

### Get Started

1. Add Attribute

``` csharp
    // TableName equals the model's Name
    [TableDescription("UserTable")]
    public class User : BaseModel
    {
        [ColumnDescription("Username")]
        public string Username { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [ColumnDescription("PasswordHash")]
        public string PasswordHash { get; set; }


        [ColumnDescription("IsActive")]
        public bool IsActive { get; set; }
    }

    // table name not equals the model's Name
    [TableDescription("ShopRouteInfo", "RouteInfoTable")]
    public class ShopRouteInfoModel:BaseModel
    {

        [ColumnDescription("RouteType，0：Area，1：Controller，2：Action")]
        public int RouteType { get; set; }


        [ColumnDescription("RouteInfoName")]
        public string RouteInfoName { get; set; }

        [ColumnDescription("RouteInfoDesc")]
        public string RouteInfoDesc { get; set; }

        [ColumnDescription("Parent RouteInfo")]
        public int ParentId { get; set; }
    }
```

2. Add custom database initializer

please refer to the samples below

3. Generate database description

``` csharp
// Generate databse description directly
new SqlServerDbDescriptionInitializer().GenerateDbDescription(context);

// Generate database description directly async
await new SqlServerDbDescriptionInitializer().GenerateDbDescriptionAsync(context);

// generate create databse description sql text
new SqlServerDbDescriptionInitializer().GenerateDbDescriptionSqlText(typeof(context));
```

### SampleProject
#### .net framework project
.net framework project url:
<https://github.com/WeihanLi/AccessControlDemo>

#### .net core project
asp.net core project:
<https://github.com/WeihanLi/AccountingApp>

### Todo
What will be done in the release V1.0

- [ ] GetTableName automatically
- [ ] Mysql database description initlizer

### Contact

You can contact me via <weihanli@outlook.com> whenever you have a problem about this project.