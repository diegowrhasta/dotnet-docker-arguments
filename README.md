# Introduction

This is a small project that aims at testing out two different ways of passing down 
some sort of argument to a docker container so that the application running on it 
changes its behavior (receives said argument).

- Passed down as a variable the moment the docker image is built.
- Through the override of env variables in the form of a `.env` file.

## Variables passed down on image build

The implementation for this is focused on the `env-file-setup` project, this is 
a simple console application that takes a flag/argument under `-n`, and the following 
argument being what changes the second part of the message that is outputted once 
the app runs.

Whilst standing at `dotnet-docker-arguments\src\dotnet-docker-arguments`. You can 
run this command:

````
docker build --build-arg APP_ARGUMENTS="-n Brother" -f ./env-file-setup/env-file-setup/Dockerfile -t env-file-setup .
````

We are passing down at build-time the arguments that will be then grabbed by the 
program once it runs on the container. Because of how the scaffold for the `Dockerfile` 
works, we have to be standing at that specific path, and also pass down to the 
`docker build` the context of `.` meaning the current path. So that it knows how to 
resolve the different folders that it copies and so on.

The other key part of this is at the `Dockerfile` level:

````
# Accept build arguments
ARG APP_ARGUMENTS="-n World"
# Set them as environment variables for runtime use
ENV APP_ARGUMENTS=$APP_ARGUMENTS

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["sh", "-c", "dotnet env-file-setup.dll $APP_ARGUMENTS"]
````

We first coalesce the arguments with `ARG APP_ARGUMENTS="-n World"` just in case we 
don't get anything from the consumer side. If we don't the default value the 
`APP_ARGUMENTS` variable will take will be `-n World`. Secondly we then assign as 
an env variable on the container this whole string. `ENV APP_ARGUMENTS=$APP_ARGUMENTS`. 
and lastly because we want to append that whole list of arguments to the program we 
do a bit of circumventing, and we execute the `dotnet` command with the _dll_ and also 
these extra arguments passed down by pipelining to a `sh -c` session that's one 
big string.

`sh -c` is a common way to run complex commands or injecting arguments down to some 
other command we want to run on a container. Hence, it's used this way, and because 
this will be a bash session, we also get the access to environment variables, hence 
doing `ENV APP_ARGUMENTS=$APP_ARGUMENTS` makes sense and when running this command 
bash will replace the value of that variable name with the actual value, that we also 
injected at the Docker image build stage.

## Env file overriding defaults

## ASP.NET Application for overriding app-settings