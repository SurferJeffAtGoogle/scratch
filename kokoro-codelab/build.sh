#!/bin/bash

# Fail on any error.
set -e
# Display commands being run.
set -x

if [ "$1" == "release" ]; then
  javac -g:none Hello.java
else
  javac Hello.java
fi