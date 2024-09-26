#!/usr/bin/env bash
sqlite3 ./chirp.db < ./schema.sql
sqlite3 ./chirp.db < ./dump.sql
