using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JDash.NetFramework.Provider.MySQL.Scripts
{
    public class TableCreationScripts
    { 
        public static string DashboardCreateScript = @"CREATE TABLE `dashboard` (
                                                          `id` int(20) NOT NULL AUTO_INCREMENT,
                                                          `appId` varchar(45) NOT NULL,
                                                          `title` nvarchar(200) DEFAULT NULL,
                                                          `shareWith` nvarchar(200) DEFAULT NULL,
                                                          `description` nvarchar(500) DEFAULT NULL,
                                                          `user` nvarchar(200) NOT NULL,
                                                          `createdAt` datetime NOT NULL,
                                                          `config` nvarchar(2000) DEFAULT NULL,
                                                          `layout` mediumtext CHARACTER SET big5,
                                                          PRIMARY KEY (`id`),
                                                          UNIQUE KEY `Id_UNIQUE` (`id`)
                                                        )
                                                    ";

        public static string DashletCreateScript = @"CREATE TABLE `dashlet` (
                                                          `id` int(20) NOT NULL AUTO_INCREMENT,
                                                          `moduleId` varchar(100) NOT NULL,
                                                          `dashboardId` bigint(20) NOT NULL,
                                                          `configuration` nvarchar(2000) DEFAULT NULL,
                                                          `title` nvarchar(200) DEFAULT NULL,
                                                          `description` nvarchar(2000) DEFAULT NULL,
                                                          `createdAt` datetime NOT NULL,
                                                          PRIMARY KEY (`id`)
                                                        )
                                                    ";

    }
}
