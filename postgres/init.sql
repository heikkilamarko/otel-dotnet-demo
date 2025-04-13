CREATE DATABASE otel_demo;

\c otel_demo;

CREATE SCHEMA demo;

CREATE TABLE demo.messages (
    id UUID PRIMARY KEY,
    name TEXT NOT NULL,
    created_at TIMESTAMPTZ NOT NULL
);
