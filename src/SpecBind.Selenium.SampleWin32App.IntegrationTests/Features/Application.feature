@mstest:DeploymentItem:TechTalk.SpecFlow.MSTest.SpecFlowPlugin.dll
Feature: Application

Scenario: Launch application
    Given I launched the application
    And I was on the Main window
    And I clicked OK

Scenario: Attach to application
    Given I launched the file "SpecBind.Selenium.SampleWin32App.exe"
    And I attached to the application
    And I was on the Main window
    When I click OK

Scenario: Launch application and check exit code
    Given I launched the file "SpecBind.Selenium.SampleWin32App.exe"
    And I attached to the application
    And I was on the Main window
    When I click OK
    Then the application should exit with code 0