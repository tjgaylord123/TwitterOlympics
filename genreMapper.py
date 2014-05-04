#!/usr/bin/env python
import sys
 
#--- get all lines from stdin ---
for line in sys.stdin:
    #--- remove leading and trailing whitespace---
    line = line.strip()

    #--- split the line into words ---
    # words = line.split()

    
    rottentoms = line
    genre = []

    for x in rottentoms['genres']:
        print '%s\t%s' % (genre, "1")
        