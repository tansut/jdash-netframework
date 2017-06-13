using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JDash.NetFramework.Provider.MsSQL.Scripts
{
    public class SQLScripts
    {
        public const string DashboardTableCreationScript = @"CREATE TABLE [{0}].dashboard (
                                                                  id int IDENTITY(1,1) NOT NULL,
                                                                  appId varchar(45) NOT NULL,
                                                                  title nvarchar(500) DEFAULT NULL,
                                                                  shareWith varchar(200) DEFAULT NULL,
                                                                  [description] varchar(500) DEFAULT NULL,
                                                                  [user] nvarchar(200) NOT NULL,
                                                                  createdAt datetime NOT NULL,
                                                                  config nvarchar(MAX) DEFAULT NULL,
                                                                  layout nvarchar(MAX) DEFAULT NULL
                                                                   CONSTRAINT [PK_Dashboard] PRIMARY KEY CLUSTERED 
                                                                (
	                                                                [id] ASC
                                                                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                                                ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";

        public const string DashletTableCreationScript = @"CREATE TABLE [{0}].dashlet (
                                                              id int IDENTITY(1,1) NOT NULL,
                                                              moduleId nvarchar(45) NOT NULL,
                                                              dashboardId int NOT NULL,
                                                              [configuration] nvarchar(MAX) DEFAULT NULL,
                                                              [title] nvarchar(500) DEFAULT NULL,
                                                              [description] nvarchar(200) DEFAULT NULL,
                                                              createdAt datetime NOT NULL,
                                                               CONSTRAINT [PK_Dashlet] PRIMARY KEY CLUSTERED 
                                                            (
	                                                            [id] ASC
                                                            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                                            ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
                                                            ";
    }
}
