--
-- PostgreSQL database dump
--

-- Dumped from database version 16.3
-- Dumped by pg_dump version 16.2

-- Started on 2024-06-01 18:31:49 UTC

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

--
-- TOC entry 230 (class 1255 OID 16384)
-- Name: calculate_distance(double precision, double precision, double precision, double precision); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.calculate_distance(lat1 double precision, lon1 double precision, lat2 double precision, lon2 double precision) RETURNS double precision
    LANGUAGE plpgsql
    AS $$
DECLARE
    x float = 69.1 * (lat2 - lat1);
    y float = 69.1 * (lon2 - lon1) * cos(lat1 / 57.3);
BEGIN
    RETURN sqrt(x * x + y * y);
END;
$$;


ALTER FUNCTION public.calculate_distance(lat1 double precision, lon1 double precision, lat2 double precision, lon2 double precision) OWNER TO postgres;

--
-- TOC entry 231 (class 1255 OID 16385)
-- Name: count_common_elements(integer[], integer[]); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.count_common_elements(arr1 integer[], arr2 integer[]) RETURNS integer
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN (
        SELECT COUNT(*)
        FROM (
            SELECT UNNEST(arr1)
            INTERSECT
            SELECT UNNEST(arr2)
        ) AS t
    );
END;
$$;


ALTER FUNCTION public.count_common_elements(arr1 integer[], arr2 integer[]) OWNER TO postgres;

--
-- TOC entry 232 (class 1255 OID 16386)
-- Name: count_shared_elements(text[], text[]); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.count_shared_elements(arr1 text[], arr2 text[]) RETURNS integer
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN (
        SELECT COUNT(*)
        FROM (
            SELECT UNNEST(arr1)
            INTERSECT
            SELECT UNNEST(arr2)
        ) AS t
    );
END;
$$;


ALTER FUNCTION public.count_shared_elements(arr1 text[], arr2 text[]) OWNER TO postgres;

--
-- TOC entry 233 (class 1255 OID 16495)
-- Name: has_user_liked(integer, integer); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.has_user_liked(liker_id integer, liked_id integer) RETURNS boolean
    LANGUAGE plpgsql
    AS $$

BEGIN
    RETURN EXISTS (SELECT 1 FROM likes WHERE liker_user_id = liker_id AND liked_user_id = liked_id);
END;
$$;


ALTER FUNCTION public.has_user_liked(liker_id integer, liked_id integer) OWNER TO postgres;

--
-- TOC entry 234 (class 1255 OID 16496)
-- Name: users_matched(integer, integer); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.users_matched(user1_id integer, user2_id integer) RETURNS boolean
    LANGUAGE plpgsql
    AS $$

BEGIN
    RETURN EXISTS (
        SELECT 1 FROM likes WHERE liker_user_id = user1_id AND liked_user_id = user2_id
    ) AND EXISTS (
        SELECT 1 FROM likes WHERE liker_user_id = user2_id AND liked_user_id = user1_id
    );
END;
$$;


ALTER FUNCTION public.users_matched(user1_id integer, user2_id integer) OWNER TO postgres;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 229 (class 1259 OID 16482)
-- Name: messages; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.messages (
    id integer NOT NULL,
    room_id integer,
    sender_id integer,
    message text,
    created_at timestamp without time zone
);


ALTER TABLE public.messages OWNER TO postgres;

--
-- TOC entry 228 (class 1259 OID 16481)
-- Name: dialogs_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.dialogs_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.dialogs_id_seq OWNER TO postgres;

--
-- TOC entry 3485 (class 0 OID 0)
-- Dependencies: 228
-- Name: dialogs_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.dialogs_id_seq OWNED BY public.messages.id;


--
-- TOC entry 215 (class 1259 OID 16389)
-- Name: interests; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.interests (
    interest_id integer NOT NULL,
    name text NOT NULL
);


ALTER TABLE public.interests OWNER TO postgres;

--
-- TOC entry 216 (class 1259 OID 16394)
-- Name: interests_interest_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.interests_interest_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.interests_interest_id_seq OWNER TO postgres;

--
-- TOC entry 3486 (class 0 OID 0)
-- Dependencies: 216
-- Name: interests_interest_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.interests_interest_id_seq OWNED BY public.interests.interest_id;


--
-- TOC entry 217 (class 1259 OID 16395)
-- Name: likes; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.likes (
    liker_user_id integer NOT NULL,
    liked_user_id integer NOT NULL
);


ALTER TABLE public.likes OWNER TO postgres;

--
-- TOC entry 218 (class 1259 OID 16398)
-- Name: pictures; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.pictures (
    picture_id integer NOT NULL,
    user_id integer,
    picture_path bytea,
    is_profile_picture integer DEFAULT 0
);


ALTER TABLE public.pictures OWNER TO postgres;

--
-- TOC entry 219 (class 1259 OID 16404)
-- Name: pictures_picture_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.pictures_picture_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.pictures_picture_id_seq OWNER TO postgres;

--
-- TOC entry 3487 (class 0 OID 0)
-- Dependencies: 219
-- Name: pictures_picture_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.pictures_picture_id_seq OWNED BY public.pictures.picture_id;


--
-- TOC entry 220 (class 1259 OID 16405)
-- Name: profile_views; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.profile_views (
    viewer_user_id integer NOT NULL,
    profile_user_id integer NOT NULL
);


ALTER TABLE public.profile_views OWNER TO postgres;

--
-- TOC entry 221 (class 1259 OID 16408)
-- Name: profiles; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.profiles (
    profile_id integer NOT NULL,
    gender text,
    age integer DEFAULT 18,
    sexual_preferences text,
    biography text,
    profile_picture_id integer,
    fame_rating integer DEFAULT 0,
    latitude real,
    longitude real,
    is_active boolean DEFAULT false NOT NULL
);


ALTER TABLE public.profiles OWNER TO postgres;

--
-- TOC entry 222 (class 1259 OID 16416)
-- Name: profiles_profile_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.profiles_profile_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.profiles_profile_id_seq OWNER TO postgres;

--
-- TOC entry 3488 (class 0 OID 0)
-- Dependencies: 222
-- Name: profiles_profile_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.profiles_profile_id_seq OWNED BY public.profiles.profile_id;


--
-- TOC entry 227 (class 1259 OID 16475)
-- Name: rooms; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.rooms (
    id integer NOT NULL,
    user1 integer,
    user2 integer
);


ALTER TABLE public.rooms OWNER TO postgres;

--
-- TOC entry 226 (class 1259 OID 16474)
-- Name: rooms_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.rooms_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.rooms_id_seq OWNER TO postgres;

--
-- TOC entry 3489 (class 0 OID 0)
-- Dependencies: 226
-- Name: rooms_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.rooms_id_seq OWNED BY public.rooms.id;


--
-- TOC entry 223 (class 1259 OID 16417)
-- Name: user_interests; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.user_interests (
    user_id integer NOT NULL,
    interest_id integer NOT NULL
);


ALTER TABLE public.user_interests OWNER TO postgres;

--
-- TOC entry 224 (class 1259 OID 16420)
-- Name: users; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.users (
    user_id integer NOT NULL,
    user_name text NOT NULL,
    first_name text NOT NULL,
    last_name text NOT NULL,
    email text NOT NULL,
    password text NOT NULL,
    last_login_at text,
    email_reset_token text,
    jwt_reset_token text,
    is_verified boolean DEFAULT false NOT NULL
);


ALTER TABLE public.users OWNER TO postgres;

--
-- TOC entry 225 (class 1259 OID 16426)
-- Name: users_user_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.users_user_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.users_user_id_seq OWNER TO postgres;

--
-- TOC entry 3490 (class 0 OID 0)
-- Dependencies: 225
-- Name: users_user_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.users_user_id_seq OWNED BY public.users.user_id;


--
-- TOC entry 3288 (class 2604 OID 16427)
-- Name: interests interest_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.interests ALTER COLUMN interest_id SET DEFAULT nextval('public.interests_interest_id_seq'::regclass);


--
-- TOC entry 3298 (class 2604 OID 16485)
-- Name: messages id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.messages ALTER COLUMN id SET DEFAULT nextval('public.dialogs_id_seq'::regclass);


--
-- TOC entry 3289 (class 2604 OID 16428)
-- Name: pictures picture_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.pictures ALTER COLUMN picture_id SET DEFAULT nextval('public.pictures_picture_id_seq'::regclass);


--
-- TOC entry 3291 (class 2604 OID 16429)
-- Name: profiles profile_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.profiles ALTER COLUMN profile_id SET DEFAULT nextval('public.profiles_profile_id_seq'::regclass);


--
-- TOC entry 3297 (class 2604 OID 16478)
-- Name: rooms id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.rooms ALTER COLUMN id SET DEFAULT nextval('public.rooms_id_seq'::regclass);


--
-- TOC entry 3295 (class 2604 OID 16430)
-- Name: users user_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users ALTER COLUMN user_id SET DEFAULT nextval('public.users_user_id_seq'::regclass);


--
-- TOC entry 3465 (class 0 OID 16389)
-- Dependencies: 215
-- Data for Name: interests; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.interests (interest_id, name) FROM stdin;
1	hah
4	lol
7	hahhah
9	st
\.


--
-- TOC entry 3467 (class 0 OID 16395)
-- Dependencies: 217
-- Data for Name: likes; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.likes (liker_user_id, liked_user_id) FROM stdin;
\.


--
-- TOC entry 3479 (class 0 OID 16482)
-- Dependencies: 229
-- Data for Name: messages; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.messages (id, room_id, sender_id, message, created_at) FROM stdin;
\.


--
-- TOC entry 3468 (class 0 OID 16398)
-- Dependencies: 218
-- Data for Name: pictures; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.pictures (picture_id, user_id, picture_path, is_profile_picture) FROM stdin;
\.


--
-- TOC entry 3470 (class 0 OID 16405)
-- Dependencies: 220
-- Data for Name: profile_views; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.profile_views (viewer_user_id, profile_user_id) FROM stdin;
\.


--
-- TOC entry 3471 (class 0 OID 16408)
-- Dependencies: 221
-- Data for Name: profiles; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.profiles (profile_id, gender, age, sexual_preferences, biography, profile_picture_id, fame_rating, latitude, longitude, is_active) FROM stdin;
9	male	18	male	amagay	6	30	40	40	t
11	male	18	male	yamashina	23	50	40	40	t
12	male	21	male	likeamerika	24	100	40	40	t
\.


--
-- TOC entry 3477 (class 0 OID 16475)
-- Dependencies: 227
-- Data for Name: rooms; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.rooms (id, user1, user2) FROM stdin;
\.


--
-- TOC entry 3473 (class 0 OID 16417)
-- Dependencies: 223
-- Data for Name: user_interests; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.user_interests (user_id, interest_id) FROM stdin;
9	1
9	4
11	1
11	4
12	7
\.


--
-- TOC entry 3474 (class 0 OID 16420)
-- Dependencies: 224
-- Data for Name: users; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.users (user_id, user_name, first_name, last_name, email, password, last_login_at, email_reset_token, jwt_reset_token, is_verified) FROM stdin;
9	grib	vova	grib	grib@mail.com	$2b$10$m26LkwHrG3jr4qHsdu3R2.T2SzI9NvZMRe3cKoAVKlBLIuMw4owUO	\N	\N	\N	t
11	mashina	ashot	mashina	mashina@mail.com	$2b$10$22u6/gjqtigm/ydVByyMKe.BNMa2MXD5Miw6wiNPdwGkHPb3EzGny	\N	\N	\N	t
12	bruno	bruno	americo	bruno@mail.com	$2b$10$noIjqJvmLaO0s.HB0MPVT.gGmV1iWmnvfncxZEw9yxmfRKOvJmQtC	\N	\N	\N	t
\.


--
-- TOC entry 3491 (class 0 OID 0)
-- Dependencies: 228
-- Name: dialogs_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.dialogs_id_seq', 1, false);


--
-- TOC entry 3492 (class 0 OID 0)
-- Dependencies: 216
-- Name: interests_interest_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.interests_interest_id_seq', 9, true);


--
-- TOC entry 3493 (class 0 OID 0)
-- Dependencies: 219
-- Name: pictures_picture_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.pictures_picture_id_seq', 24, true);


--
-- TOC entry 3494 (class 0 OID 0)
-- Dependencies: 222
-- Name: profiles_profile_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.profiles_profile_id_seq', 1, false);


--
-- TOC entry 3495 (class 0 OID 0)
-- Dependencies: 226
-- Name: rooms_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.rooms_id_seq', 2, true);


--
-- TOC entry 3496 (class 0 OID 0)
-- Dependencies: 225
-- Name: users_user_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.users_user_id_seq', 20, true);


--
-- TOC entry 3320 (class 2606 OID 16489)
-- Name: messages dialogs_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.messages
    ADD CONSTRAINT dialogs_pkey PRIMARY KEY (id);


--
-- TOC entry 3300 (class 2606 OID 16434)
-- Name: interests interests_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.interests
    ADD CONSTRAINT interests_pkey PRIMARY KEY (interest_id);


--
-- TOC entry 3302 (class 2606 OID 16436)
-- Name: likes likes_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.likes
    ADD CONSTRAINT likes_pkey PRIMARY KEY (liker_user_id, liked_user_id);


--
-- TOC entry 3304 (class 2606 OID 16438)
-- Name: pictures pictures_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.pictures
    ADD CONSTRAINT pictures_pkey PRIMARY KEY (picture_id);


--
-- TOC entry 3306 (class 2606 OID 16440)
-- Name: profile_views profile_views_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.profile_views
    ADD CONSTRAINT profile_views_pkey PRIMARY KEY (viewer_user_id, profile_user_id);


--
-- TOC entry 3308 (class 2606 OID 16442)
-- Name: profiles profiles_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.profiles
    ADD CONSTRAINT profiles_pkey PRIMARY KEY (profile_id);


--
-- TOC entry 3318 (class 2606 OID 16480)
-- Name: rooms rooms_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.rooms
    ADD CONSTRAINT rooms_pkey PRIMARY KEY (id);


--
-- TOC entry 3310 (class 2606 OID 16444)
-- Name: user_interests user_interests_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.user_interests
    ADD CONSTRAINT user_interests_pkey PRIMARY KEY (user_id, interest_id);


--
-- TOC entry 3312 (class 2606 OID 16446)
-- Name: users users_email_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_email_key UNIQUE (email);


--
-- TOC entry 3314 (class 2606 OID 16448)
-- Name: users users_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (user_id);


--
-- TOC entry 3316 (class 2606 OID 16450)
-- Name: users users_user_name_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_user_name_key UNIQUE (user_name);


--
-- TOC entry 3321 (class 2606 OID 16490)
-- Name: messages dialogs_room_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.messages
    ADD CONSTRAINT dialogs_room_id_fkey FOREIGN KEY (room_id) REFERENCES public.rooms(id);


-- Completed on 2024-06-01 18:31:50 UTC

--
-- PostgreSQL database dump complete
--

