@mstest:DeploymentItem:TechTalk.SpecFlow.MSTest.SpecFlowPlugin.dll
Feature: Window

Scenario: Wait for window to display
    Given I launched the application
    And I waited for the Main window

Scenario: Click button in main window
    Given I launched the application
    And I was on the Main window
    And I clicked Button1

Scenario: Ensure child dialog is displayed
    Given I launched the application
    And I was on the Main window
    When I click Display child dialog
    And I am on the Child dialog

Scenario: Click button in child dialog
    Given I launched the application
    And I was on the Main window
    And I clicked Display child dialog
    And I was on the Child dialog
    And I clicked OK

Scenario: Click button in child dialog and then in main window
    Given I launched the application
    And I was on the Main window
    And I clicked Display child dialog
    And I was on the Child dialog
    And I clicked OK
    And I was on the Main window
    And I clicked OK

Scenario: Close main window
    Given I launched the application
    And I was on the Main window
    When I click OK
