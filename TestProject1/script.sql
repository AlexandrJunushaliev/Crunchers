-- SEQUENCE: public."Categories_CategoryId_seq"

-- DROP SEQUENCE public."Categories_CategoryId_seq";

CREATE SEQUENCE public."Categories_CategoryId_seq"
    INCREMENT 1
    START 25
    MINVALUE 1
    MAXVALUE 2147483647
    CACHE 1;

ALTER SEQUENCE public."Categories_CategoryId_seq"
    OWNER TO postgres;
-- SEQUENCE: public."Characteristics_CharacteristicId_seq"

-- DROP SEQUENCE public."Characteristics_CharacteristicId_seq";

CREATE SEQUENCE public."Characteristics_CharacteristicId_seq"
    INCREMENT 1
    START 30
    MINVALUE 1
    MAXVALUE 2147483647
    CACHE 1;

ALTER SEQUENCE public."Characteristics_CharacteristicId_seq"
    OWNER TO postgres;
-- SEQUENCE: public."City_CityId_seq"

-- DROP SEQUENCE public."City_CityId_seq";

CREATE SEQUENCE public."City_CityId_seq"
    INCREMENT 1
    START 7
    MINVALUE 1
    MAXVALUE 2147483647
    CACHE 1;

ALTER SEQUENCE public."City_CityId_seq"
    OWNER TO postgres;
-- SEQUENCE: public."Filters_FilterId_seq"

-- DROP SEQUENCE public."Filters_FilterId_seq";

CREATE SEQUENCE public."Filters_FilterId_seq"
    INCREMENT 1
    START 18
    MINVALUE 1
    MAXVALUE 2147483647
    CACHE 1;

ALTER SEQUENCE public."Filters_FilterId_seq"
    OWNER TO postgres;
-- SEQUENCE: public."Marks_markid_seq"

-- DROP SEQUENCE public."Marks_markid_seq";

CREATE SEQUENCE public."Marks_markid_seq"
    INCREMENT 1
    START 12
    MINVALUE 1
    MAXVALUE 2147483647
    CACHE 1;

ALTER SEQUENCE public."Marks_markid_seq"
    OWNER TO postgres;
-- SEQUENCE: public."Orders_OrderId_seq"

-- DROP SEQUENCE public."Orders_OrderId_seq";

CREATE SEQUENCE public."Orders_OrderId_seq"
    INCREMENT 1
    START 12
    MINVALUE 1
    MAXVALUE 2147483647
    CACHE 1;

ALTER SEQUENCE public."Orders_OrderId_seq"
    OWNER TO postgres;
-- SEQUENCE: public."Photos_PhotoId_seq"

-- DROP SEQUENCE public."Photos_PhotoId_seq";

CREATE SEQUENCE public."Photos_PhotoId_seq"
    INCREMENT 1
    START 23
    MINVALUE 1
    MAXVALUE 2147483647
    CACHE 1;

ALTER SEQUENCE public."Photos_PhotoId_seq"
    OWNER TO postgres;
-- SEQUENCE: public."PointsOfPickUp_PointId_seq"

-- DROP SEQUENCE public."PointsOfPickUp_PointId_seq";

CREATE SEQUENCE public."PointsOfPickUp_PointId_seq"
    INCREMENT 1
    START 8
    MINVALUE 1
    MAXVALUE 2147483647
    CACHE 1;

ALTER SEQUENCE public."PointsOfPickUp_PointId_seq"
    OWNER TO postgres;

-- SEQUENCE: public."Products_ProductId_seq"

-- DROP SEQUENCE public."Products_ProductId_seq";

CREATE SEQUENCE public."Products_ProductId_seq"
    INCREMENT 1
    START 25
    MINVALUE 1
    MAXVALUE 2147483647
    CACHE 1;

ALTER SEQUENCE public."Products_ProductId_seq"
    OWNER TO postgres;

-- SEQUENCE: public."Reviews_ReviewId_seq"

-- DROP SEQUENCE public."Reviews_ReviewId_seq";

CREATE SEQUENCE public."Reviews_ReviewId_seq"
    INCREMENT 1
    START 8
    MINVALUE 1
    MAXVALUE 2147483647
    CACHE 1;

ALTER SEQUENCE public."Reviews_ReviewId_seq"
    OWNER TO postgres;
CREATE TABLE public."Categories"
(
    "CategoryId" integer NOT NULL DEFAULT nextval('"Categories_CategoryId_seq"'::regclass),
    "CategoryName" character varying(255) COLLATE pg_catalog."default",
    CONSTRAINT "Categories_pkey" PRIMARY KEY ("CategoryId")
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."Categories"
    OWNER to postgres;

-- Index: "Categories"_"CategoryName"_uindex

-- DROP INDEX public."""Categories""_""CategoryName""_uindex";

CREATE UNIQUE INDEX """Categories""_""CategoryName""_uindex"
    ON public."Categories" USING btree
    ("CategoryName" COLLATE pg_catalog."default")
    TABLESPACE pg_default;

-- Table: public."Characteristics"

-- DROP TABLE public."Characteristics";

CREATE TABLE public."Characteristics"
(
    "CharacteristicId" integer NOT NULL DEFAULT nextval('"Characteristics_CharacteristicId_seq"'::regclass),
    "CategoryId" integer,
    "CharacteristicType" character varying(25) COLLATE pg_catalog."default",
    "CharacteristicName" character varying(255) COLLATE pg_catalog."default",
    "Unit" character varying(20) COLLATE pg_catalog."default" DEFAULT ''::character varying,
    CONSTRAINT "Characteristics_pkey" PRIMARY KEY ("CharacteristicId"),
    CONSTRAINT categoryid_fk FOREIGN KEY ("CategoryId")
        REFERENCES public."Categories" ("CategoryId") MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."Characteristics"
    OWNER to postgres;

-- Table: public."Products"

-- DROP TABLE public."Products";

CREATE TABLE public."Products"
(
    "ProductId" integer NOT NULL DEFAULT nextval('"Products_ProductId_seq"'::regclass),
    "CategoryId" integer,
    "ProductName" character varying(255) COLLATE pg_catalog."default",
    "ProductPrice" integer,
    "RatingsSum" integer,
    "RatingsAmount" integer,
    CONSTRAINT "Products_pkey" PRIMARY KEY ("ProductId"),
    CONSTRAINT categotyid_fk FOREIGN KEY ("CategoryId")
        REFERENCES public."Categories" ("CategoryId") MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."Products"
    OWNER to postgres;

-- Table: public."CharacteristicValues"

-- DROP TABLE public."CharacteristicValues";

CREATE TABLE public."CharacteristicValues"
(
    "ProductId" integer NOT NULL,
    "CharacteristicId" integer NOT NULL,
    "ValueReal" real,
    "ValueString" character varying(255) COLLATE pg_catalog."default",
    CONSTRAINT "CharacteristicValues_pkey" PRIMARY KEY ("ProductId", "CharacteristicId"),
    CONSTRAINT """CharacteristicId""_fk" FOREIGN KEY ("CharacteristicId")
        REFERENCES public."Characteristics" ("CharacteristicId") MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE,
    CONSTRAINT """ProductId""_fk" FOREIGN KEY ("ProductId")
        REFERENCES public."Products" ("ProductId") MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."CharacteristicValues"
    OWNER to postgres;

-- Table: public."Filters"

-- DROP TABLE public."Filters";

CREATE TABLE public."Filters"
(
    "FilterId" integer NOT NULL DEFAULT nextval('"Filters_FilterId_seq"'::regclass),
    "CharacteristicId" integer,
    "From" real,
    "To" real,
    CONSTRAINT "Filters_pkey" PRIMARY KEY ("FilterId"),
    CONSTRAINT """Filters""_Characteristics__fk" FOREIGN KEY ("CharacteristicId")
        REFERENCES public."Characteristics" ("CharacteristicId") MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."Filters"
    OWNER to postgres;

-- Table: public."Images"

-- DROP TABLE public."Images";

CREATE TABLE public."Images"
(
    "ImageId" integer NOT NULL DEFAULT nextval('"Photos_PhotoId_seq"'::regclass),
    "ImageLink" character varying(255) COLLATE pg_catalog."default",
    "ProductId" integer,
    "ImageRole" character varying(15) COLLATE pg_catalog."default",
    CONSTRAINT "Images_pkey" PRIMARY KEY ("ImageId"),
    CONSTRAINT productid_fk FOREIGN KEY ("ProductId")
        REFERENCES public."Products" ("ProductId") MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."Images"
    OWNER to postgres;

-- Table: public."Cities"

-- DROP TABLE public."Cities";

CREATE TABLE public."Cities"
(
    "CityId" integer NOT NULL DEFAULT nextval('"City_CityId_seq"'::regclass),
    "NameRu" character varying(150) COLLATE pg_catalog."default",
    CONSTRAINT "City_pkey" PRIMARY KEY ("CityId")
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."Cities"
    OWNER to postgres;

-- Index: cities_nameru_uindex

-- DROP INDEX public.cities_nameru_uindex;

CREATE UNIQUE INDEX cities_nameru_uindex
    ON public."Cities" USING btree
    ("NameRu" COLLATE pg_catalog."default")
    TABLESPACE pg_default;


-- Table: public."Users"

-- DROP TABLE public."Users";

CREATE TABLE public."Users"
(
    "UserId" character varying(128) COLLATE pg_catalog."default" NOT NULL,
    "FullName" character varying(100) COLLATE pg_catalog."default",
    "CityId" integer,
    CONSTRAINT "User_pk" PRIMARY KEY ("UserId"),
    CONSTRAINT cityid_fk FOREIGN KEY ("CityId")
        REFERENCES public."Cities" ("CityId") MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE SET NULL
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."Users"
    OWNER to postgres;


-- Table: public."PointsOfPickUp"

-- DROP TABLE public."PointsOfPickUp";

CREATE TABLE public."PointsOfPickUp"
(
    "PointId" integer NOT NULL DEFAULT nextval('"PointsOfPickUp_PointId_seq"'::regclass),
    "CityId" integer,
    "Address" character varying(255) COLLATE pg_catalog."default",
    CONSTRAINT "PointsOfPickUp_pkey" PRIMARY KEY ("PointId"),
    CONSTRAINT """PointsOfPickUp""_fk" FOREIGN KEY ("CityId")
        REFERENCES public."Cities" ("CityId") MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."PointsOfPickUp"
    OWNER to postgres;

-- Table: public."Reviews"

-- DROP TABLE public."Reviews";

CREATE TABLE public."Reviews"
(
    "ReviewId" integer NOT NULL DEFAULT nextval('"Reviews_ReviewId_seq"'::regclass),
    "UserName" character varying(128) COLLATE pg_catalog."default",
    "ReviewText" character varying(350) COLLATE pg_catalog."default",
    "ReviewDate" timestamp without time zone,
    "ProductId" integer,
    CONSTRAINT "Reviews_pkey" PRIMARY KEY ("ReviewId"),
    CONSTRAINT """ProductId""___fk" FOREIGN KEY ("ProductId")
        REFERENCES public."Products" ("ProductId") MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."Reviews"
    OWNER to postgres;

-- Table: public."ShoppingCarts"

-- DROP TABLE public."ShoppingCarts";

CREATE TABLE public."ShoppingCarts"
(
    "CartJson" jsonb,
    "UserId" character varying(128) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT shoppingcarts_pk PRIMARY KEY ("UserId")
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."ShoppingCarts"
    OWNER to postgres;
-- FUNCTION: public.process_product_rating_change()

-- DROP FUNCTION public.process_product_rating_change();

CREATE FUNCTION public.process_product_rating_change()
    RETURNS trigger
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE NOT LEAKPROOF
AS $BODY$
BEGIN
    --
    -- Добавление строки в emp_audit, которая отражает операцию, выполняемую в emp;
    -- для определения типа операции применяется специальная переменная TG_OP.
    --
    IF (TG_OP = 'DELETE') THEN
        UPDATE "Products" SET "RatingsSum"="RatingsSum"-old."Mark","RatingsAmount"="RatingsAmount"-1 where "ProductId"=old."ProductId";
        return old;
    ELSIF (TG_OP = 'UPDATE') THEN
        UPDATE "Products" SET "RatingsSum"="RatingsSum"-old."Mark"+new."Mark" where "ProductId"=old."ProductId";
        return new;
    ELSIF (TG_OP = 'INSERT') THEN
        UPDATE "Products" SET "RatingsSum"="RatingsSum"+new."Mark","RatingsAmount"="RatingsAmount"+1 where "ProductId"=new."ProductId";
        return new;
    END IF;
    RETURN NULL; -- возвращаемое значение для триггера AFTER игнорируется
END;
$BODY$;

ALTER FUNCTION public.process_product_rating_change()
    OWNER TO postgres;
-- Table: public."Marks"

-- DROP TABLE public."Marks";

CREATE TABLE public."Marks"
(
    "UserName" character varying(128) COLLATE pg_catalog."default",
    "ProductId" integer,
    "Mark" integer,
    "MarkId" integer NOT NULL DEFAULT nextval('"Marks_markid_seq"'::regclass),
    CONSTRAINT "Marks_pkey" PRIMARY KEY ("MarkId"),
    CONSTRAINT """ProductId""___fk" FOREIGN KEY ("ProductId")
        REFERENCES public."Products" ("ProductId") MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."Marks"
    OWNER to postgres;

-- Trigger: productsratingtrigger

-- DROP TRIGGER productsratingtrigger ON public."Marks";

CREATE TRIGGER productsratingtrigger
    BEFORE INSERT OR DELETE OR UPDATE
    ON public."Marks"
    FOR EACH ROW
    EXECUTE PROCEDURE public.process_product_rating_change();

-- Table: public."Orders"

-- DROP TABLE public."Orders";

CREATE TABLE public."Orders"
(
    "OrderId" integer NOT NULL DEFAULT nextval('"Orders_OrderId_seq"'::regclass),
    "Active" boolean,
    "Delivered" boolean,
    "Paid" boolean,
    "IsForPickUp" boolean,
    "Price" integer,
    "ComfortTimeFrom" timestamp without time zone,
    "Address" character varying(128) COLLATE pg_catalog."default",
    "Name" character varying(48) COLLATE pg_catalog."default",
    "Phone" character varying(15) COLLATE pg_catalog."default",
    "Email" character varying(128) COLLATE pg_catalog."default",
    "ComfortTimeTo" timestamp without time zone,
    CONSTRAINT "Orders_pkey" PRIMARY KEY ("OrderId")
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."Orders"
    OWNER to postgres;

-- Table: public."ProductsFromOrders"

-- DROP TABLE public."ProductsFromOrders";

CREATE TABLE public."ProductsFromOrders"
(
    "ProductId" integer NOT NULL,
    "OrderId" integer NOT NULL,
    CONSTRAINT "ProductsFromOrders_pkey" PRIMARY KEY ("ProductId", "OrderId"),
    CONSTRAINT orders_fk FOREIGN KEY ("OrderId")
        REFERENCES public."Orders" ("OrderId") MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE,
    CONSTRAINT products_fk FOREIGN KEY ("ProductId")
        REFERENCES public."Products" ("ProductId") MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."ProductsFromOrders"
    OWNER to postgres;