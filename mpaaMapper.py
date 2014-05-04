#!/usr/bin/env python
import sys
 
#--- get all lines from stdin ---
for line in sys.stdin:
    #--- remove leading and trailing whitespace---
    line = line.strip()

    #--- split the line into words ---
    # words = line.split()

    
    nyt_results = line['results']
    res = nyt_results[0]
    mpaa = res['mpaa_rating']

	print '%s\t%s' % (mpaa, "1")



    #--- output tuples [word, 1] in tab-delimited format---
    #for word in words: 
        #print '%s\t%s' % (word, "1")