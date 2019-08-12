/**
 * *************************************************
 * Copyright (c) 2019, Grindrod Bank Limited
 * License MIT: https://opensource.org/licenses/MIT
 * **************************************************
 */
﻿namespace Scrubfu.Tests
{
    public static class Constants
    {
        public const string CommandLineTemplate = @"--log_level info {0} {1}";
        public const string CommandLineTemplateWithLog = @"--log {0} --log_level debug {1} {2}";

        public const string SQL_STATEMENT_COLUMN_NAME_COMMENT_1 = "COMMENT ON COLUMN public.employees.email IS '~MA:3,2;#;@,.~';";
        public const string SQL_STATEMENT_COLUMN_NAME_COMMENT_2 = "COMMENT ON COLUMN public.customers.phone IS '~FZ:~';";
        public const string SQL_STATEMENT_COLUMN_NAME_COMMENT_3 = "COMMENT ON COLUMN public.customers.address IS '';";
        public const string SQL_STATEMENT_COLUMN_NAME_COMMENT_4 = "COMMENT ON COLUMN public.\"Persons\".\"FavouriteFoods\" IS '--~MA[2]:1,1;$~';";
        public const string SQL_STATEMENT_COLUMN_NAME_COMMENT_5 = "COMMENT ON COLUMN public.\"Persons\".\"FavouriteFoods\" IS '';";

        public const string SQL_STATEMENT_INSERT = @"INSERT public.customers (customer_id, company_name, contact_name, contact_title, address, city, region, postal_code, country, phone, fax) VALUES ('DUMON', 'Du monde entier', 'Janine Labrune', 'Owner', '67, rue des Cinquante Otages', 'Nantes', NULL, '44000', 'France', '40.67.88.88', '40.67.89.89');";
        public const string SQL_STATEMENT_INSERT_INTO = @"INSERT INTO public.customers (customer_id, company_name, contact_name, contact_title, address, city, region, postal_code, country, phone, fax) VALUES ('DUMON', 'Du monde entier', 'Janine Labrune', 'Owner', '67, rue des Cinquante Otages', 'Nantes', NULL, '44000', 'France', '40.67.88.88', '40.67.89.89');";
        public const string SQL_STATEMENT_COPY = @"COPY public.customers (customer_id, company_name, contact_name, contact_title, address, city, region, postal_code, country, phone, fax) FROM stdin;";
        public const string SQL_STATEMENT_COPY_VALUELINE = @"DUMON	Du monde entier	Janine Labrune	Owner	67, rue des Cinquante Otages	Nantes	\N	44000	France	40.67.88.88	40.67.89.89";

        public const string SQL_STATEMENT_CREATE_TABLE = "CREATE TABLE public.customers (\r\n    customer_id bpchar NOT NULL,\r\n    company_name character varying(40) NOT NULL,\r\n    contact_name character varying(30),\r\n    contact_title character varying(30),\r\n    address character varying(60),\r\n    city character varying(15),\r\n    region character varying(15),\r\n    postal_code character varying(10),\r\n    country character varying(15),\r\n    phone character varying(24),\r\n    fax character varying(24)\r\n);";

        public const string SQL_STATEMENT_CREATE_TABLE_COLUMN_DEFINITION_1 = "customer_id bpchar NOT NULL";
        public const string SQL_STATEMENT_CREATE_TABLE_COLUMN_DEFINITION_2 = "company_name character varying(40) NOT NULL";
        public const string SQL_STATEMENT_CREATE_TABLE_COLUMN_DEFINITION_3 = "contact_name character varying(30)";
        public const string SQL_STATEMENT_CREATE_TABLE_COLUMN_DEFINITION_4 = "contact_title character varying(30)";
        public const string SQL_STATEMENT_CREATE_TABLE_COLUMN_DEFINITION_5 = "address character varying(60)";
        public const string SQL_STATEMENT_CREATE_TABLE_COLUMN_DEFINITION_6 = "city character";
        public const string SQL_STATEMENT_CREATE_TABLE_COLUMN_DEFINITION_7 = "region character";
        public const string SQL_STATEMENT_CREATE_TABLE_COLUMN_DEFINITION_8 = "postal_code text";
        public const string SQL_STATEMENT_CREATE_TABLE_COLUMN_DEFINITION_9 = "country character varying(15)";
        public const string SQL_STATEMENT_CREATE_TABLE_COLUMN_DEFINITION_10 = "phone character varying(24)";
        public const string SQL_STATEMENT_CREATE_TABLE_COLUMN_DEFINITION_11 = "\"Id\" uuid DEFAULT public.uuid_generate_v4() NOT NULL";
        public const string SQL_STATEMENT_CREATE_TABLE_COLUMN_DEFINITION_12 = "employee_id smallint NOT NULL";
        public const string SQL_STATEMENT_CREATE_TABLE_COLUMN_DEFINITION_13 = "\"LuckyNumbers\" smallint[]";
        public const string SQL_STATEMENT_CREATE_TABLE_COLUMN_DEFINITION_14 = "\"FavouriteFoods\" text[]";
        public const string SQL_STATEMENT_CREATE_TABLE_COLUMN_DEFINITION_15 = "\"Phone\" text";

        public const int COPY_TEST_SAMPLE_TEST_LINE_NUMBER = 39;
        public const int INSERT_TEST_SAMPLE_TEST_LINE_NUMBER = 38;

        public const int SCRUBFU_TAG_COUNT_IN_NORTHWIND_COPY_SAMPLE = 4;
        public const int SCRUBFU_TAG_COUNT_IN_NORTHWIND_INSERTS_SAMPLE = 4;
    }
}