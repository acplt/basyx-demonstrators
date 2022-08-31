# Workflows

This folder contains BPMN processes to be deployed in the Camunda engine.

## Requirements
* Java Runtime Environment (JRE) and JAVA_HOME environment variable set

## Manual install instruction for Camunda

* Download [Camunda](https://camunda.com/download/) OpenSource Communiy Edition
* Follow installation steps
* Delete example folder with its content (~\camunda-bpm-run-7.17.0\internal\example)
* Copy bpmn-file ([](workflow_1_provideCan.bpmn)) into folder ~\camunda-bpm-run-7.17.0\configuration\resources\  

* Open Webbrowser [http://localhost:8080/](http://localhost:8080/)
* Login information at:  ~\camunda-bpm-run-7.17.0\configuration\default.yml
    * Login: demo 
    * Password: demo