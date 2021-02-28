@mstest:DeploymentItem:TechTalk.SpecFlow.MSTest.SpecFlowPlugin.dll
Feature: Keyboard

Scenario: Enter text in an edit box
    Given I launched the application
    And I was on the Main window
    And I entered data
     | Field           | Value       |
     | ThisIsAnEditBox | sample text |
