/**
 * *************************************************
 * Copyright (c) 2019, Grindrod Bank Limited
 * License MIT: https://opensource.org/licenses/MIT
 * **************************************************
 */
﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Scrubfu.Extensions;

namespace Scrubfu.Tests
{
    public static class TestHelpers
    {
        private static string GetCurrentBaseDirectory()
        {
            return string.Concat(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Path.DirectorySeparatorChar);
        }

        public static string GetOutputFilePath()
        {
            return MakeFullFilePath($"{GetCurrentBaseDirectory()}");
        }

        public static string GetCurrentPath()
        {
            return MakeFullFilePath($"{GetCurrentBaseDirectory()}");
        }

        public static string BuildCommand(string inputFilePath, string outputFilePath)
        {
            return string.Format(Constants.CommandLineTemplate, inputFilePath, outputFilePath);
        }

        public static string BuildCommand(string inputFilePath, string outputFilePath, string logFilePath)
        {
            return string.Format(Constants.CommandLineTemplateWithLog, logFilePath, inputFilePath, outputFilePath);
        }
        public static string[] GetArgs(string command)
        {
            return command.Split(" ");
        }

        public static string[] BuildCommandArgs(string inputFilePath, string outputFilePath)
        {
            return GetArgs(BuildCommand(inputFilePath, outputFilePath));
        }

        public static string[] BuildCommandArgs(string inputFilePath, string outputFilePath, string logFilePath)
        {
            return GetArgs(BuildCommand(inputFilePath, outputFilePath, logFilePath));
        }

       public static void PrepareOutputFile(string outputFilePath)
        {
            // Create output folder if it does not already exists
            string outDir = Path.GetDirectoryName(outputFilePath);
            if (!Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);

            // Clear output file if it exists
            if (File.Exists(outputFilePath))
                File.Delete(outputFilePath);

        }

        public static void GenerateSamplePGDumpFile(string filename, bool UseCopy, Dictionary<string, string> scrubComments = null)
        {
            if (File.Exists(filename))
                File.Delete(filename);

            File.AppendAllText(filename,
                string.Concat("CREATE TABLE public.employees (\r\n",
                            "employee_id smallint NOT NULL,\r\n",
                            "last_name character varying(20) NOT NULL,\r\n",
                            "first_name character varying(10) NOT NULL,\r\n",
                            "title character varying(30),\r\n",
                            "title_of_courtesy character varying(25),\r\n",
                            "birth_date date,\r\n",
                            "hire_date date,\r\n",
                            "address character varying(60),\r\n",
                            "city character varying(15),\r\n",
                            "region character varying(15),\r\n",
                            "postal_code character varying(10),\r\n",
                            "country character varying(15),\r\n",
                            "home_phone character varying(24),\r\n",
                            "extension character varying(4),\r\n",
                            "photo bytea,\r\n",
                            "notes text,\r\n",
                            "reports_to smallint,\r\n",
                            "photo_path character varying(255),\r\n",
                            "email text,\r\n",
                            ");\r\n"));

            File.AppendAllText(filename, "\r\n");

            if (scrubComments != null)
            {
                foreach (KeyValuePair<string, string> comment in scrubComments)
                    File.AppendAllText(filename, $"COMMENT ON COLUMN {comment.Key} IS '{comment.Value}';\r\n");
            }

            File.AppendAllText(filename, "\r\n");

            if (UseCopy)
            {
                File.AppendAllText(filename,
                    string.Concat("COPY public.employees(employee_id, last_name, first_name, title, title_of_courtesy, birth_date, hire_date, address, city, region, postal_code, country, home_phone, extension, photo, notes, reports_to, photo_path, email) FROM stdin;\r\n",
                                    "1\tDavolio\tNancy\tSales Representative\tMs.\t1948-12-08\t1992-05-01\t507 - 20th Ave.E.\\nApt. 2A\tSeattle\tWA\t98122\tUSA\t(206) 555-9857\t5467\t\\x\tEducation includes a BA in psychology from Colorado State University in 1970.  She also completed The Art of the Cold Call.Nancy is a member of Toastmasters International.\t2\thttp://accweb/emmployees/davolio.bmp\tnancy@acme.com\r\n",
                                    "2\tFuller\tAndrew\tVice President, Sales\tDr.\t1952-02-19\t1992-08-14\t908 W.Capital Way\tTacoma\tWA\t98401\tUSA\t(206) 555-9482\t3457\t\\x\tAndrew received his BTS commercial in 1974 and a Ph.D. in international marketing from the University of Dallas in 1981.  He is fluent in French and Italian and reads German.He joined the company as a sales representative, was promoted to sales manager in January 1992 and to vice president of sales in March 1993.  Andrew is a member of the Sales Management Roundtable, the Seattle Chamber of Commerce, and the Pacific Rim Importers Association.\t\\N\thttp://accweb/emmployees/fuller.bmp\tandrew.f@acme.com\r\n",
                                    "3\tLeverling\tJanet\tSales Representative\tMs.\t1963-08-30\t1992-04-01\t722 Moss Bay Blvd.\tKirkland\tWA\t98033\tUSA\t(206) 555-3412\t3355\t\\x\tJanet has a BS degree in chemistry from Boston College(1984).  She has also completed a certificate program in food retailing management.Janet was hired as a sales associate in 1991 and promoted to sales representative in February 1992.\t2\thttp://accweb/emmployees/leverling.bmp\tj.leverling@acmeco.net\r\n",
                                    "4\tPeacock\tMargaret\tSales Representative\tMrs.\t1937-09-19\t1993-05-03\t4110 Old Redmond Rd.\tRedmond\tWA\t98052\tUSA\t(206) 555-8122\t5176\t\\x\tMargaret holds a BA in English literature from Concordia College(1958) and an MA from the American Institute of Culinary Arts(1966).  She was assigned to the London office temporarily from July through November 1992.\t2\thttp://accweb/emmployees/peacock.bmp\tMargaretPeacock@acme.co.uk\r\n",
                                    "5\tBuchanan\tSteven\tSales Manager\tMr.\t1955-03-04\t1993-10-17\t14 Garrett Hill\tLondon\t\\N\tSW1 8JR\tUK\t(71) 555-4848\t3453\t\\x\tSteven Buchanan graduated from St.Andrews University, Scotland, with a BSC degree in 1976.  Upon joining the company as a sales representative in 1992, he spent 6 months in an orientation program at the Seattle office and then returned to his permanent post in London.He was promoted to sales manager in March 1993.  Mr.Buchanan has completed the courses Successful Telemarketing and International Sales Management.He is fluent in French.\t2\thttp://accweb/emmployees/buchanan.bmp\tsb@acmeco.us\r\n",
                                    "6\tSuyama\tMichael\tSales Representative\tMr.\t1963-07-02\t1993-10-17\tCoventry House\\nMiner Rd.\tLondon\t\\N\tEC2 7JR\tUK\t(71) 555-7773\t428\t\\x\tMichael is a graduate of Sussex University(MA, economics, 1983) and the University of California at Los Angeles(MBA, marketing, 1986).  He has also taken the courses Multi-Cultural Selling and Time Management for the Sales Professional.He is fluent in Japanese and can read and write French, Portuguese, and Spanish.\t5\thttp://accweb/emmployees/davolio.bmp\tmichael@suyamasales.com\r\n",
                                    "7\tKing\tRobert\tSales Representative\tMr.\t1960-05-29\t1994-01-02\tEdgeham Hollow\\nWinchester Way\tLondon\t\\N\tRG1 9SP\tUK\t(71) 555-5598\t465\t\\x\tRobert King served in the Peace Corps and traveled extensively before completing his degree in English at the University of Michigan in 1992, the year he joined the company.After completing a course entitled Selling in Europe, he was transferred to the London office in March 1993.\t5\thttp://accweb/emmployees/davolio.bmp\tRobert.King@acme.com\r\n",
                                    "8\tCallahan\tLaura\tInside Sales Coordinator\tMs.\t1958-01-09\t1994-03-05\t4726 - 11th Ave. N.E.\tSeattle\tWA\t98105\tUSA\t(206) 555-1189\t2344\t\\x\tLaura received a BA in psychology from the University of Washington.She has also completed a course in business French.  She reads and writes French.\t2\thttp://accweb/emmployees/davolio.bmp\tlaura.callahan@acme.co.uk\r\n",
                                    "9\tDodsworth\tAnne\tSales Representative\tMs.\t1966-01-27\t1994-11-15\t7 Houndstooth Rd.\tLondon\t\\N\tWG2 7LT\tUK\t(71) 555-4444\t452\t\\x\tAnne has a BA degree in English from St.Lawrence College.She is fluent in French and German.\t5\thttp://accweb/emmployees/davolio.bmp\ta.dodsworth@acme.com\r\n",
                                    "\\.\r\n"));
            }
            else
            {
                File.AppendAllText(filename,
                string.Concat("INSERT INTO public.employees(employee_id, last_name, first_name, title, title_of_courtesy, birth_date, hire_date, address, city, region, postal_code, country, home_phone, extension, photo, notes, reports_to, photo_path, email) VALUES(1, 'Davolio', 'Nancy', 'Sales Representative', 'Ms.', '1948-12-08', '1992-05-01', '507 - 20th Ave. E.\nApt. 2A', 'Seattle', 'WA', '98122', 'USA', '(206) 555-9857', '5467', '\\x', 'Education includes a BA in psychology...', 2, 'http://accweb/emmployees/davolio.bmp', 'nancy@acme.com');\r\n",
                                            "INSERT INTO public.employees(employee_id, last_name, first_name, title, title_of_courtesy, birth_date, hire_date, address, city, region, postal_code, country, home_phone, extension, photo, notes, reports_to, photo_path, email) VALUES(2, 'Fuller', 'Andrew', 'Vice President, Sales', 'Dr.', '1952-02-19', '1992-08-14', '908 W. Capital Way', 'Tacoma', 'WA', '98401', 'USA', '(206) 555-9482', '3457', '\\x', 'Andrew received his BTS commercial in 1974 and a Ph.D. in ...', NULL, 'http://accweb/emmployees/fuller.bmp', 'andrew.f@acme.com');\r\n",
                                            "INSERT INTO public.employees(employee_id, last_name, first_name, title, title_of_courtesy, birth_date, hire_date, address, city, region, postal_code, country, home_phone, extension, photo, notes, reports_to, photo_path, email) VALUES(3, 'Leverling', 'Janet', 'Sales Representative', 'Ms.', '1963-08-30', '1992-04-01', '722 Moss Bay Blvd.', 'Kirkland', 'WA', '98033', 'USA', '(206) 555-3412', '3355', '\\x', 'Janet has a BS degree in chemistry from Boston College (1984)...', 2, 'http://accweb/emmployees/leverling.bmp', 'j.leverling@acmeco.net');\r\n",
                                            "INSERT INTO public.employees(employee_id, last_name, first_name, title, title_of_courtesy, birth_date, hire_date, address, city, region, postal_code, country, home_phone, extension, photo, notes, reports_to, photo_path, email) VALUES(4, 'Peacock', 'Margaret', 'Sales Representative', 'Mrs.', '1937-09-19', '1993-05-03', '4110 Old Redmond Rd.', 'Redmond', 'WA', '98052', 'USA', '(206) 555-8122', '5176', '\\x', 'Margaret holds a BA in English literature from Concordia College (1958)...', 2, 'http://accweb/emmployees/peacock.bmp', 'MargaretPeacock@acme.co.uk');\r\n",
                                            "INSERT INTO public.employees(employee_id, last_name, first_name, title, title_of_courtesy, birth_date, hire_date, address, city, region, postal_code, country, home_phone, extension, photo, notes, reports_to, photo_path, email) VALUES(5, 'Buchanan', 'Steven', 'Sales Manager', 'Mr.', '1955-03-04', '1993-10-17', '14 Garrett Hill', 'London', NULL, 'SW1 8JR', 'UK', '(71) 555-4848', '3453', '\\x', 'Steven Buchanan graduated from St. Andrews University...', 2, 'http://accweb/emmployees/buchanan.bmp', 'sb@acmeco.us');\r\n",
                                            "INSERT INTO public.employees(employee_id, last_name, first_name, title, title_of_courtesy, birth_date, hire_date, address, city, region, postal_code, country, home_phone, extension, photo, notes, reports_to, photo_path, email) VALUES(6, 'Suyama', 'Michael', 'Sales Representative', 'Mr.', '1963-07-02', '1993-10-17', 'Coventry House\nMiner Rd.', 'London', NULL, 'EC2 7JR', 'UK', '(71) 555-7773', '428', '\\x', 'Michael is a graduate of Sussex University (MA, economics, 1983) and the University of California...', 5, 'http://accweb/emmployees/davolio.bmp', 'michael@suyamasales.com');\r\n",
                                            "INSERT INTO public.employees(employee_id, last_name, first_name, title, title_of_courtesy, birth_date, hire_date, address, city, region, postal_code, country, home_phone, extension, photo, notes, reports_to, photo_path, email) VALUES(7, 'King', 'Robert', 'Sales Representative', 'Mr.', '1960-05-29', '1994-01-02', 'Edgeham Hollow\nWinchester Way', 'London', NULL, 'RG1 9SP', 'UK', '(71) 555-5598', '465', '\\x', 'Robert King served in the Peace Corps and traveled extensively before completing his degree in English at the University of Michigan in 1992...', 5, 'http://accweb/emmployees/davolio.bmp', 'Robert.King@acme.com');\r\n",
                                            "INSERT INTO public.employees(employee_id, last_name, first_name, title, title_of_courtesy, birth_date, hire_date, address, city, region, postal_code, country, home_phone, extension, photo, notes, reports_to, photo_path, email) VALUES(8, 'Callahan', 'Laura', 'Inside Sales Coordinator', 'Ms.', '1958-01-09', '1994-03-05', '4726 - 11th Ave. N.E.', 'Seattle', 'WA', '98105', 'USA', '(206) 555-1189', '2344', '\\x', 'Laura received a BA in psychology from the University of Washington...', 2, 'http://accweb/emmployees/davolio.bmp', 'laura.callahan@acme.co.uk');\r\n",
                                            "INSERT INTO public.employees(employee_id, last_name, first_name, title, title_of_courtesy, birth_date, hire_date, address, city, region, postal_code, country, home_phone, extension, photo, notes, reports_to, photo_path, email) VALUES(9, 'Dodsworth', 'Anne', 'Sales Representative', 'Ms.', '1966-01-27', '1994-11-15', '7 Houndstooth Rd.', 'London', NULL, 'WG2 7LT', 'UK', '(71) 555-4444', '452', '\\x', 'Anne has a BA degree in English from St. Lawrence College.  She is fluent in French and German.', 5, 'http://accweb/emmployees/davolio.bmp', 'a.dodsworth@acme.com');\r\n"));

            }
        }

        public static string MakeFullFilePath(string path)
        {
            if (!Path.IsPathRooted(path))
                path = Path.GetFullPath(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + path);

            return path;
        }

    }
}
