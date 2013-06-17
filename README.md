CheckService
============ 

Now we have the F5 local traffic manager in front of REST services it can be difficult to figure out if a particular instance of a service on a host is working properly.

This is a little program that might make this easier: it performs an HTTP GET on a service on a specific host and returns the HTTP status code together with the XML returned by the service.

For example, suppose you'd like to see if Offerings is running properly in CQ1:

    C:\> checkservice  http://offerings.music.cq1.brislabs.com/0.4/status

    200 OK from HTTP GET to http://offerings.music.cq1.brislabs.com/0.4/status

    <?xml version="1.0" encoding="utf-8"?><status serviceName="Offerings" version="1.2.4.0" recursive="false" success="true" xmlns="http://music.ovi.com/1.0/status" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"><statusItem name="OfferingsDB" success="true"/><statusItem name="Reporting" success="true"/><statusItem name="Logging" success="true"/></status>


And to check if the instance on host CQ1APIIIS7001.brislabs.com is running:

    C:\> checkservice http://offerings.music.cq1.brislabs.com/0.4/status -d cq1apiis7001.brislabs.com

    200 OK from HTTP GET to http://offerings.music.cq1.brislabs.com/0.4/status on cq1apiis7001.brislabs.com

    <?xml version="1.0" encoding="utf-8"?><status serviceName="Offerings" version="1.2.4.0" recursive="false" success="true" xmlns="http://music.ovi.com/1.0/status" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"><statusItem name="OfferingsDB" success="true"/><statusItem name="Reporting" success="true"/><statusItem name="Logging" success="true"/></status>

Changelog
---------

1.0.2   Support sites with duff SSL certificates (and moan about them, too)

1.0.1   Allow specification of alternate ports, e.g. http://foo.bah:8080/

1.0.0   Initial version


Enjoy,

Jeremy McGee, Nokia Music SCM Tools

mailto:jeremy.mcgee@bassettdata.com
