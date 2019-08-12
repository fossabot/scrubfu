--
-- PostgreSQL database dump
--

-- Dumped from database version 10.9
-- Dumped by pg_dump version 11.4

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_with_oids = false;

--
-- Name: customers; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.customers (
    customer_id bpchar NOT NULL,
    company_name character varying(40) NOT NULL,
    contact_name character varying(30),
    contact_title character varying(30),
    address character varying(60),
    city character varying(15),
    region character varying(15),
    postal_code character varying(10),
    country character varying(15),
    phone character varying(24),
    fax character varying(24)
);


ALTER TABLE public.customers OWNER TO postgres;

--
-- Name: COLUMN customers.address; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON COLUMN public.customers.address IS '~FZ~';


--
-- Name: COLUMN customers.city; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON COLUMN public.customers.city IS '~RE:Berlin,nilreB;''London'',''nodnoL'';''Mannheim'',''miehnnaM''~~MA:3,2;#;@,.~';


--
-- Name: COLUMN customers.phone; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON COLUMN public.customers.phone IS '~RA:+NNN (NNN) NNN-NNNN~';

--
-- Name: array_test; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.array_test (
    name text,
    pay_by_quarter integer[],
    schedule text[]
);


ALTER TABLE public.array_test OWNER TO postgres;

--
-- Name: COLUMN array_test.pay_by_quarter; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON COLUMN public.array_test.pay_by_quarter IS '~RA:NNNNN~';


--
-- Name: COLUMN array_test.schedule; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON COLUMN public.array_test.schedule IS '~FZ~';

--
-- Data for Name: customers; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.customers (customer_id, company_name, contact_name, contact_title, address, city, region, postal_code, country, phone, fax) FROM stdin;
ALFKI	Alfreds Futterkiste	Maria Anders	Sales Representative	!@#$%^&*()-_,./\\ Obere Str. 57	Berlin	\N	12209	Germany	030-0074321	030-0076545
ANATR	Ana Trujillo Emparedados y helados	Ana Trujillo	Owner	Avda. de la Constitución 2222	México D.F.	\N	05021	Mexico	(5) 555-4729	(5) 555-3745
ANTON	Antonio Moreno Taquería	Antonio Moreno	Owner	Mataderos  2312	México D.F.	\N	05023	Mexico	(5) 555-3932	\N
AROUT	Around the Horn	Thomas Hardy	Sales Representative	120 Hanover Sq.	London	\N	WA1 1DP	UK	(171) 555-7788	(171) 555-6750
BERGS	Berglunds snabbköp	Christina Berglund	Order Administrator	Berguvsvägen  8	Luleå	\N	S-958 22	Sweden	0921-12 34 65	0921-12 34 67
BLAUS	Blauer See Delikatessen	Hanna Moos	Sales Representative	Forsterstr. 57	Mannheim	\N	68306	Germany	0621-08460	0621-08924
BLONP	Blondesddsl père et fils	Frédérique Citeaux	Marketing Manager	24, place Kléber	Strasbourg	\N	67000	France	88.60.15.31	88.60.15.32
\.

--
-- Data for Name: array_test; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.array_test (name, pay_by_quarter, schedule) FROM stdin;
Bill	{10000,10000,10000,10000}	{{meeting,lunch},{training,presentation}}
Carol	{20000,25000,25000,25000}	{{breakfast,consulting},{meeting,lunch}}
\.



--
-- Name: customers pk_customers; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.customers
    ADD CONSTRAINT pk_customers PRIMARY KEY (customer_id);


--
-- PostgreSQL database dump complete
--

