using NUnit.Framework;
using NUnit.Framework.Legacy;
using FluentAssertions;

namespace HomeExercise.Tasks.ObjectComparison;

public class ObjectComparison
{
    [Test]
    [Description("Проверка текущего царя")]
    [Category("ToRefactor")]
    public void CheckCurrentTsar()
    {
        var actualTsar = TsarRegistry.GetCurrentTsar();

        var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
            new Person("Vasili III of Russia", 28, 170, 60, null));

        actualTsar.Should().BeEquivalentTo(expectedTsar, options => options.Excluding(ctx => ctx.Name == nameof(Person.Id)
                                                                                          && ctx.DeclaringType == typeof(Person)));
    }

    [Test]
    [Description("Альтернативное решение. Какие у него недостатки?")]
    public void CheckCurrentTsar_WithCustomEquality()
    {
        var actualTsar = TsarRegistry.GetCurrentTsar();
        var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
            new Person("Vasili III of Russia", 28, 170, 60, null));

        /* 
        1. Непонятно, где конкретно и что не сошлось, плохая информативность теста. В сводке по тесту просто будет
           текст по типу "Expected: true, but: false", FluentAssertions в этом плане дают полную сводку полей;
        2. После рефакторинга и добавления нового функционала в Person придется переписывать тест,
           если конечно вообще про него не забудете. Методы FluentAssertions в этом плане надежнее,
           ведь проходятся по всем полям;
        3. Не вижу смысла придумывать велосипед, за тебя уже все сделали и этим пользуется большая часть разрабов.
         */
        ClassicAssert.True(AreEqual(actualTsar, expectedTsar));
    }

    private bool AreEqual(Person? actual, Person? expected)
    {
        if (actual == expected) return true;
        if (actual == null || expected == null) return false;
        return
            // вот тут можно забыть добавить что-либо, очень тяжко воспринимать подобные колонны
            actual.Name == expected.Name
            && actual.Age == expected.Age
            && actual.Height == expected.Height
            && actual.Weight == expected.Weight
            && AreEqual(actual.Parent, expected.Parent);
    }
}
