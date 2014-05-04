#!/usr/bin/env python
import sys
 
#--- get all lines from stdin ---
for line in sys.stdin:
    #--- remove leading and trailing whitespace---
    line = line.strip()

    #--- split the line into words ---
    # words = line.split()

    
    newyorktimes = line
    nyt_results = newyorktimes['results']
	res = nyt_results[0]
	# title = res['display_title']
	thousand = res['thousand_best']
	# print thousand

	print '%s\t%s' % (thousand, "1")



    #--- output tuples [word, 1] in tab-delimited format---
    #for word in words: 
        #print '%s\t%s' % (word, "1")