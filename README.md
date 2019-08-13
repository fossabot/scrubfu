[![CircleCI](https://circleci.com/gh/GrindrodBank/scrubfu.svg?style=svg)](https://circleci.com/gh/GrindrodBank/scrubfu)[![FOSSA Status](https://app.fossa.io/api/projects/git%2Bgithub.com%2FGrindrodBank%2Fscrubfu.svg?type=shield)](https://app.fossa.io/projects/git%2Bgithub.com%2FGrindrodBank%2Fscrubfu?ref=badge_shield)

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=scrubfu&metric=alert_status)](https://sonarcloud.io/dashboard?id=scrubfu)

# Scrubfu

## Introduction

Production data should never leave a production environment and yet developers need access to coherent data sets to properly develop and test.  

scrubfu is a .NET CLI tool that makes creating usable development data from production data a predictable, audit-able and repeatable process. 

It is compatible with Postgres databases and utilizes the COMMENT's on table columns to indicate the type of scrubbing & obfuscation required.  

``` bash
Usage: scrubfu [OPTIONS] [INFILE] [OUTFILE]

  [INFILE] is the input file obtained by a pg_dump.
           This can also be standard out stream.

  [OUTFILE] is the scrubfu'ed file, ready to be imported with pg_import.
            This can also be the standard out stream.

Options:
  -h, --help                      Show this message and exit.
  -v, --version                   Show the version and exit.
  --log TEXT                      Optional LOGFILE, defaults to standard out.
  --log_level [error|info|debug]  Used with [--log=LOGFILE].
```

## Quick Start

### Step 1: Clone the scrubfu repository

```bash
git clone git@github.com:GrindrodBank/scrubfu.git

cd scrubfu
```

### Step 2: Get scrubfu and other tools

Log into the Artifactory Docker Repo with your username and password, and pull scrubfu:

```bash
docker login

docker pull grindrodbank/scrubfu
```

Get the PostgreSQL client tools:

```bash
apt-get install postgresql-client

# Record PSQL Version
PSQLVERSION=`psql --version | awk '{print $3}'`
```

### Step 2: Get PostgreSQL and load sample file

Get the scrubfu_sample.sql file:

```bash
cp doc/scrubfu_sample.sql /tmp/.
```

Start a PostgreSQL Docker container.

```bash
DBPS=`docker run -e POSTGRES_PASSWORD=pgpass -d -p 15432:5432 postgres:$PSQLVERSION`

# Confirm the container ID
echo $DBPS
```

Create the scrubfu_sample database and import the sample data:

```bash
PGPASSWORD=pgpass createdb -h localhost -p 15432 -U postgres scrubfu_sample

PGPASSWORD=pgpass psql -h localhost -p 15432 -U postgres scrubfu_sample < /tmp/scrubfu_sample.sql 

# Confirm the tables have been imported
PGPASSWORD=pgpass psql -h localhost -p 15432 -U postgres scrubfu_sample -c "\dt"

# Dump the database out to SQL file again for later comparison
PGPASSWORD=pgpass pg_dump -h localhost -p 15432 -U postgres scrubfu_sample > /tmp/scrubfu_sample.sql
```

### Step 3a: Scrub in line with pg_dump

This option is ideal for scrubbing a `pg_dump` process output in-line.

Pipe the `pg_dump` and the `scrubfu` commands.

```bash
PGPASSWORD=pgpass pg_dump -h localhost -p 15432 -U postgres scrubfu_sample | docker run --rm -a stdin -a stdout -i grindrodbank/scrubfu > /tmp/scrubfu_sample_scrubbed.sql

# Confirm output
cat /tmp/scrubfu_sample_scrubbed.sql | less
```

### Step 3b: Scrub export file

This option scrubs an already dumped file into a new file.

```bash
PGPASSWORD=pgpass pg_dump -h localhost -p 15432 -U postgres --file /tmp/scrubfu_sample.sql scrubfu_sample

docker run --rm -v /tmp:/tmp -i grindrodbank/scrubfu /tmp/scrubfu_sample.sql /tmp/scrubfu_sample_scrubbed.sql

# Confirm output
cat /tmp/scrubfu_sample_scrubbed.sql | less
```

### Step 3c: Other operations (optional)

The following commands show how to perform other operations.

Import a scrubbed file:

```bash
export PGPASSWORD="pgpass"

dropdb -h localhost -p 15432 -U postgres --if-exists scrubfu_sample_scrubbed

createdb -h localhost -p 15432 -U postgres scrubfu_sample_scrubbed

psql -h localhost -p 15432 -U postgres scrubfu_sample_scrubbed --file /tmp/scrubfu_sample_scrubbed.sql

# Confirm output
cat /tmp/scrubfu_sample_scrubbed.sql | less
```

Map a log file to local storage, and set log level to debug:

```bash
touch /tmp/scrubfu.log

docker run --rm -v /tmp:/tmp -v /tmp/scrubfu.log:/tmp/scrubfu.log -i grindrodbank/scrubfu --log /tmp/scrubfu.log --log_level debug /tmp/scrubfu_sample.sql /tmp/scrubfu_sample_scrubbed.sql

# Confirm log file output
cat /tmp/scrubfu.log | less
```

Perform an end to end export -> scrub -> import without writing any files:

```bash
export PGPASSWORD="pgpass"

dropdb -h localhost -p 15432 -U postgres --if-exists scrubfu_sample_scrubbed

createdb -h localhost -p 15432 -U postgres scrubfu_sample_scrubbed

pg_dump -h localhost -p 15432 -U postgres scrubfu_sample | docker run --rm -a stdin -a stdout -i grindrodbank/scrubfu | psql -h localhost -p 15432 -U postgres scrubfu_sample_scrubbed --file -

# Confirm the tables have been imported
PGPASSWORD=pgpass psql -h localhost -p 15432 -U postgres scrubfu_sample_scrubbed -c "select * from array_test;" 
```

### Step 4: Cleanup

Stop docker containers:

```bash
docker stop $DBPS

docker rm $DBPS
```

## Project Documentation

All project documentation is currently available within the /doc folder.

- [User Guide](doc/user-guide.md)
- [Contributing to scrubfu](doc/contributing.md) 
- [Contribution Workflow](doc/contribution-workflow.md) 
- [Coding Style Guide](doc/coding-style.md) 
- [Roadmap](doc/roadmap.md)
- [Copyright](doc/copyright.md)

---
&copy; Copyright 2019, Grindrod Bank Limited, and distributed under the MIT License.


## License
[![FOSSA Status](https://app.fossa.io/api/projects/git%2Bgithub.com%2FGrindrodBank%2Fscrubfu.svg?type=large)](https://app.fossa.io/projects/git%2Bgithub.com%2FGrindrodBank%2Fscrubfu?ref=badge_large)