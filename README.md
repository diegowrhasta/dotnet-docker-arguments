# Introduction

This is a small project that aims at testing out two different ways of passing down 
some sort of argument to a docker container so that the application running on it 
changes its behavior (receives said argument).

- Passed down as a variable the moment the docker image is built.
- Through the override of env variables in the form of a `.env` file.

## Variables passed down on image build

The implementation for this is focused on the `build-arguments-setup` project, this is 
a simple console application that takes a flag/argument under `-n`, and the following 
argument being what changes the second part of the message that is outputted once 
the app runs.

Whilst standing at `dotnet-docker-arguments\src\dotnet-docker-arguments`. You can 
run this command:

````
docker build --build-arg APP_ARGUMENTS="-n World" -f ./build-arguments-setup/Dockerfile -t build-arguments-setup .
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
ENTRYPOINT ["sh", "-c", "dotnet build-arguments-setup.dll $APP_ARGUMENTS"]
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

### Running the Docker container

A good way to test the container after building it is to simply run it like so: 
`docker run --rm build-arguments-setup`, this way the container once it exits, it will 
just delete itself, and you don't have remnants at all.

## Env file overriding defaults

Okay, so there's some level of miss-information, docker-compose is the one leveraging 
a `.env` file a lot, it uses that in an intermediate step when resolving all the 
containers, the values and stuff like that. It's great still to know that it leverages 
it that way.

_Advise: Do not put sensitive information in ARGs that are declared in Dockerfiles_. 

They leave a trace so people can easily extract secrets, passwords, keys or whatever 
if they know what they are doing.

Again, we have to buld the image with the known constraints, while standing at 
`dotnet-docker-arguments\src\dotnet-docker-arguments`. We can run the 

````
docker build -f ./env-file-setup/Dockerfile -t env-file-setup .
````

command, notice how we aren't passing any ARG variable to it, ARG variables are things 
that can be injected into a Dockerfile the moment that the `docker build` command is 
run. Remember that.

In our `env-file-setup` project we have simply copied the same code as the previous 
project so the behavior is the same, but the way that the arguments are injected are 
changed by environment variables that can easily be passed down when running the 
`docker run`. There are two ways of doing this, you can run either 
of these commands and either of them will result in the same:

````
docker run --env-file .env env-file-setup

docker run -e APP_ARGUMENTS="-n Universe" env-file-setup
````

If we have at the current standing directory an `.env` file it will parse all the 
key-value pairs and then have them injected into the container the moment it boots up.

This is an example of an `.env` file that's at the project level:

````
APP_ARGUMENTS=-n Universe
````

And this is how our Dockerfile looks like:

````
# Declare environment variable with default value (coalesce)
ENV APP_ARGUMENTS="-n World"

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["sh", "-c", "dotnet env-file-setup.dll $APP_ARGUMENTS"]
````

So by standing somewhere that has a `.env` file we can run `docker run --env-file .env env-file-setup` 
and then have the message overriden by the corresponding value, this is great.

## ASP.NET Application for overriding app-settings